import gateway from '../client/gatewayClient.js';
import logger  from '../utils/logger.js';
import { t, msg } from '../i18n/index.js';

export const projectToolDefinitions = [
  {
    name: 'projekt_kereses',
    description: t('projekt_kereses').description,
    inputSchema: {
      type: 'object',
      properties: {
        CategoryId:  { type: 'integer', description: t('projekt_kereses').CategoryId },
        StatusId:    { type: 'integer', description: t('projekt_kereses').StatusId },
        ContactId:   { type: 'integer', description: t('projekt_kereses').ContactId },
        UserId:      { type: 'integer', description: t('projekt_kereses').UserId },
        Name:        { type: 'string',  description: t('projekt_kereses').Name },
        Page:        { type: 'integer', description: t('projekt_kereses').Page,        default: 1 },
        MaxElements: { type: 'integer', description: t('projekt_kereses').MaxElements, default: 100 },
      },
    },
  },
  {
    name: 'projekt_lekeres',
    description: t('projekt_lekeres').description,
    inputSchema: {
      type: 'object',
      properties: {
        Id: { type: 'integer', description: t('projekt_lekeres').Id },
      },
      required: ['Id'],
    },
  },
  {
    name: 'projekt_letrehozas',
    description: t('projekt_letrehozas').description,
    inputSchema: {
      type: 'object',
      properties: {
        CategoryId:  { type: 'integer', description: t('projekt_letrehozas').CategoryId },
        ContactId:   { type: 'integer', description: t('projekt_letrehozas').ContactId },
        Name:        { type: 'string',  description: t('projekt_letrehozas').Name },
        UserId:      { type: 'integer', description: t('projekt_letrehozas').UserId },
        Description: { type: 'string',  description: t('projekt_letrehozas').Description },
      },
      required: ['CategoryId'],
    },
  },
  {
    name: 'projekt_statusz_valtas',
    description: t('projekt_statusz_valtas').description,
    inputSchema: {
      type: 'object',
      properties: {
        Id:       { type: 'integer', description: t('projekt_statusz_valtas').Id },
        StatusId: { type: 'integer', description: t('projekt_statusz_valtas').StatusId },
      },
      required: ['Id', 'StatusId'],
    },
  },
];

export async function handleProjectTool(name, args) {
  switch (name) {
    case 'projekt_kereses': {
      logger.info('projekt_kereses', args);
      const res = await gateway.get('/api/projects', { params: args });
      return formatProjectList(res.data);
    }
    case 'projekt_lekeres': {
      logger.info('projekt_lekeres', { id: args.Id });
      const res = await gateway.get(`/api/projects/${args.Id}`);
      return formatProject(res.data);
    }
    case 'projekt_letrehozas': {
      logger.info('projekt_letrehozas', args);
      const res = await gateway.post('/api/projects', args);
      return `${msg.projectCreated}\n${formatProject(res.data)}`;
    }
    case 'projekt_statusz_valtas': {
      logger.info('projekt_statusz_valtas', { id: args.Id, statusId: args.StatusId });
      const res = await gateway.patch(`/api/projects/${args.Id}/status`, { StatusId: args.StatusId });
      return `${msg.statusChanged}\n${formatProject(res.data)}`;
    }
    default:
      throw new Error(msg.unknownTool(name));
  }
}

function formatProject(p) {
  if (!p) return msg.noData;
  return [
    `**${p.Name || '(unnamed)'}** (ID: ${p.Id})`,
    p.CategoryName ? `📁 ${p.CategoryName}` : null,
    p.StatusName   ? `📊 ${p.StatusName}`   : null,
    p.ContactName  ? `👤 ${p.ContactName}`  : null,
    p.UserName     ? `🙋 ${p.UserName}`     : null,
    p.CreatedAt    ? `📅 ${p.CreatedAt}`    : null,
    p.Description  ? `📝 ${p.Description}` : null,
  ].filter(Boolean).join('\n');
}

function formatProjectList(data) {
  const results = data?.Results;
  if (!results || Object.keys(results).length === 0) return msg.noProject;
  const items = Object.values(results);
  return (
    msg.projectsFound(items.length) + '\n' +
    items.map(p =>
      `• ID ${p.Id}: **${p.Name || '(unnamed)'}**` +
      (p.StatusName  ? ` [${p.StatusName}]`  : '') +
      (p.ContactName ? ` – ${p.ContactName}` : '')
    ).join('\n')
  );
}
