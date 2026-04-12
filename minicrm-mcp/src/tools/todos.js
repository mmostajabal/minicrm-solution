import gateway from '../client/gatewayClient.js';
import logger  from '../utils/logger.js';
import { t, msg } from '../i18n/index.js';

export const todoToolDefinitions = [
  {
    name: 'teendo_letrehozas',
    description: t('teendo_letrehozas').description,
    inputSchema: {
      type: 'object',
      properties: {
        ProjectId: { type: 'integer', description: t('teendo_letrehozas').ProjectId },
        UserId:    { type: 'integer', description: t('teendo_letrehozas').UserId },
        Type: {
          type: 'string',
          description: t('teendo_letrehozas').Type,
          enum: ['Call', 'Meeting', 'Email', 'Task', 'Deadline'],
          default: 'Task',
        },
        Deadline: { type: 'string', description: t('teendo_letrehozas').Deadline },
        Comment:  { type: 'string', description: t('teendo_letrehozas').Comment },
      },
      required: ['ProjectId', 'UserId'],
    },
  },
  {
    name: 'teendo_lekeres',
    description: t('teendo_lekeres').description,
    inputSchema: {
      type: 'object',
      properties: {
        CardId: { type: 'integer', description: t('teendo_lekeres').CardId },
      },
      required: ['CardId'],
    },
  },
];

export async function handleTodoTool(name, args) {
  switch (name) {
    case 'teendo_letrehozas': {
      logger.info('teendo_letrehozas', args);
      const res = await gateway.post('/api/todos', args);
      return `${msg.todoCreated}\n${formatTodo(res.data)}`;
    }
    case 'teendo_lekeres': {
      logger.info('teendo_lekeres', { cardId: args.CardId });
      const res = await gateway.get(`/api/todos/${args.CardId}`);
      return formatTodoList(res.data);
    }
    default:
      throw new Error(msg.unknownTool(name));
  }
}

function formatTodo(t) {
  if (!t) return msg.noData;
  const statusLabel = t.Status === 1 ? msg.statusDone : msg.statusOpen;
  return [
    `**${t.Type || 'Todo'}** (ID: ${t.Id}) ${statusLabel}`,
    t.Deadline ? `⏰ ${t.Deadline}` : null,
    t.UserName ? `👤 ${t.UserName}` : null,
    t.Comment  ? `📝 ${t.Comment}` : null,
  ].filter(Boolean).join('\n');
}

function formatTodoList(data) {
  const results = data?.Results;
  if (!results || results.length === 0) return msg.noTodo;
  return (
    msg.todosFound(results.length) + '\n' +
    results.map(item => {
      const icon = item.Status === 1 ? '✅' : '🔄';
      return `${icon} **${item.Type || 'Task'}** – ${item.Comment || ''}` +
        (item.Deadline ? ` | ⏰ ${item.Deadline}` : '') +
        (item.UserName ? ` | 👤 ${item.UserName}` : '');
    }).join('\n')
  );
}
