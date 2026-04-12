import gateway from '../client/gatewayClient.js';
import logger  from '../utils/logger.js';
import { t, msg } from '../i18n/index.js';

export const invoiceSchemaToolDefinitions = [
  {
    name: 'szamla_lekerdezes',
    description: t('szamla_lekerdezes').description,
    inputSchema: {
      type: 'object',
      properties: {
        ProjectId:   { type: 'integer', description: t('szamla_lekerdezes').ProjectId },
        ContactId:   { type: 'integer', description: t('szamla_lekerdezes').ContactId },
        Status: {
          type: 'string',
          description: t('szamla_lekerdezes').Status,
          enum: ['Paid', 'Unpaid', 'Overdue'],
        },
        Page:        { type: 'integer', description: t('szamla_lekerdezes').Page,        default: 1 },
        MaxElements: { type: 'integer', description: t('szamla_lekerdezes').MaxElements, default: 100 },
      },
    },
  },
  {
    name: 'schema_lekerdezes',
    description: t('schema_lekerdezes').description,
    inputSchema: {
      type: 'object',
      properties: {
        Type: {
          type: 'string',
          description: t('schema_lekerdezes').Type,
          default: 'categories',
        },
      },
    },
  },
];

export async function handleInvoiceSchemaTool(name, args) {
  switch (name) {
    case 'szamla_lekerdezes': {
      logger.info('szamla_lekerdezes', args);
      const res = await gateway.get('/api/invoices', { params: args });
      return formatInvoiceList(res.data);
    }
    case 'schema_lekerdezes': {
      const type = args.Type || 'categories';
      logger.info('schema_lekerdezes', { type });
      const url = type === 'categories' ? '/api/schema/categories' : `/api/schema/${type}`;
      const res = await gateway.get(url);
      return `${msg.schemaHeader(type)}\n\`\`\`json\n${JSON.stringify(res.data, null, 2)}\n\`\`\``;
    }
    default:
      throw new Error(msg.unknownTool(name));
  }
}

function formatInvoiceList(data) {
  const results = data?.Results;
  if (!results || results.length === 0) return msg.noInvoice;
  return (
    msg.invoicesFound(results.length) + '\n' +
    results.map(inv => {
      const icon = inv.Status === 'Paid' ? '✅' : inv.Status === 'Overdue' ? '❗' : '⏳';
      return `${icon} **${inv.InvoiceNumber || 'Invoice'}** – ` +
        `${inv.Amount ? `${inv.Amount} ${inv.Currency || 'HUF'}` : '?'} [${inv.Status || ''}]` +
        (inv.DueDate     ? ` | ${inv.DueDate}`     : '') +
        (inv.ContactName ? ` | ${inv.ContactName}` : '');
    }).join('\n')
  );
}
