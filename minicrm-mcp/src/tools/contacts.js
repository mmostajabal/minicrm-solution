import gateway from '../client/gatewayClient.js';
import logger  from '../utils/logger.js';
import { t, msg } from '../i18n/index.js';

// ── Tool definitions ──────────────────────────────────────────────────────────

export const contactToolDefinitions = [
  {
    name: 'kontakt_kereses',
    description: t('kontakt_kereses').description,
    inputSchema: {
      type: 'object',
      properties: {
        Name:        { type: 'string',  description: t('kontakt_kereses').Name },
        Email:       { type: 'string',  description: t('kontakt_kereses').Email },
        Phone:       { type: 'string',  description: t('kontakt_kereses').Phone },
        StatusId:    { type: 'integer', description: t('kontakt_kereses').StatusId },
        UserId:      { type: 'integer', description: t('kontakt_kereses').UserId },
        Page:        { type: 'integer', description: t('kontakt_kereses').Page,        default: 1 },
        MaxElements: { type: 'integer', description: t('kontakt_kereses').MaxElements, default: 100 },
      },
    },
  },
  {
    name: 'kontakt_lekeres',
    description: t('kontakt_lekeres').description,
    inputSchema: {
      type: 'object',
      properties: {
        Id: { type: 'integer', description: t('kontakt_lekeres').Id },
      },
      required: ['Id'],
    },
  },
  {
    name: 'kontakt_letrehozas',
    description: t('kontakt_letrehozas').description,
    inputSchema: {
      type: 'object',
      properties: {
        FirstName:   { type: 'string', description: t('kontakt_letrehozas').FirstName },
        LastName:    { type: 'string', description: t('kontakt_letrehozas').LastName },
        Email:       { type: 'string', description: t('kontakt_letrehozas').Email },
        Phone:       { type: 'string', description: t('kontakt_letrehozas').Phone },
        Type:        { type: 'string', description: t('kontakt_letrehozas').Type, enum: ['Person', 'Company'] },
        Description: { type: 'string', description: t('kontakt_letrehozas').Description },
        Position:    { type: 'string', description: t('kontakt_letrehozas').Position },
      },
      required: ['LastName'],
    },
  },
  {
    name: 'kontakt_modositas',
    description: t('kontakt_modositas').description,
    inputSchema: {
      type: 'object',
      properties: {
        Id:          { type: 'integer', description: t('kontakt_modositas').Id },
        FirstName:   { type: 'string',  description: t('kontakt_modositas').FirstName },
        LastName:    { type: 'string',  description: t('kontakt_modositas').LastName },
        Email:       { type: 'string',  description: t('kontakt_modositas').Email },
        Phone:       { type: 'string',  description: t('kontakt_modositas').Phone },
        Description: { type: 'string',  description: t('kontakt_modositas').Description },
        Position:    { type: 'string',  description: t('kontakt_modositas').Position },
      },
      required: ['Id'],
    },
  },
];

// ── Handlers ──────────────────────────────────────────────────────────────────

export async function handleContactTool(name, args) {
  switch (name) {
    case 'kontakt_kereses': {
      logger.info('kontakt_kereses', args);
      const res = await gateway.get('/api/contacts', { params: args });
      return formatContactList(res.data);
    }
    case 'kontakt_lekeres': {
      logger.info('kontakt_lekeres', { id: args.Id });
      const res = await gateway.get(`/api/contacts/${args.Id}`);
      return formatContact(res.data);
    }
    case 'kontakt_letrehozas': {
      logger.info('kontakt_letrehozas', args);
      const res = await gateway.post('/api/contacts', args);
      return `${msg.contactCreated}\n${formatContact(res.data)}`;
    }
    case 'kontakt_modositas': {
      const { Id, ...fields } = args;
      logger.info('kontakt_modositas', { Id, fields });
      const res = await gateway.put(`/api/contacts/${Id}`, fields);
      return `${msg.contactUpdated}\n${formatContact(res.data)}`;
    }
    default:
      throw new Error(msg.unknownTool(name));
  }
}

// ── Formatters ────────────────────────────────────────────────────────────────

function formatContact(c) {
  if (!c) return msg.noData;
  return [
    `**${c.LastName || ''} ${c.FirstName || ''}** (ID: ${c.Id})`,
    c.Email       ? `📧 ${c.Email}`           : null,
    c.Phone       ? `📞 ${c.Phone}`           : null,
    c.Position    ? `💼 ${c.Position}`        : null,
    c.StatusName  ? `📊 ${c.StatusName}`      : null,
    c.UserName    ? `👤 ${c.UserName}`        : null,
    c.Description ? `📝 ${c.Description}`    : null,
  ].filter(Boolean).join('\n');
}

function formatContactList(data) {
  const results = data?.Results;
  if (!results || Object.keys(results).length === 0) return msg.noContact;
  const items = Object.values(results);
  return (
    msg.contactsFound(items.length) + '\n' +
    items.map(c =>
      `• ID ${c.Id}: **${c.LastName || ''} ${c.FirstName || ''}**` +
      (c.Email ? ` – ${c.Email}` : '') +
      (c.Phone ? ` – ${c.Phone}` : '')
    ).join('\n')
  );
}
