/**
 * miniCRM MCP Server
 * ──────────────────
 * Local Claude Desktop MCP server for individual miniCRM users.
 * Exposes 12 CRM tools over stdio transport with bilingual support (hu/en).
 *
 * Start:   node src/index.js
 * Config:  .env file (see .env.example)
 */

import 'dotenv/config';
import { Server }             from '@modelcontextprotocol/sdk/server/index.js';
import { StdioServerTransport } from '@modelcontextprotocol/sdk/server/stdio.js';
import {
  CallToolRequestSchema,
  ListToolsRequestSchema
} from '@modelcontextprotocol/sdk/types.js';

import logger from './utils/logger.js';
import { msg }  from './i18n/index.js';
import { contactToolDefinitions, handleContactTool }         from './tools/contacts.js';
import { projectToolDefinitions, handleProjectTool }         from './tools/projects.js';
import { todoToolDefinitions, handleTodoTool }               from './tools/todos.js';
import { invoiceSchemaToolDefinitions, handleInvoiceSchemaTool } from './tools/invoicesSchema.js';

// ── All 12 tools ──────────────────────────────────────────────────────────────
const ALL_TOOLS = [
  ...contactToolDefinitions,       // kontakt_kereses, kontakt_lekeres, kontakt_letrehozas, kontakt_modositas
  ...projectToolDefinitions,       // projekt_kereses, projekt_lekeres, projekt_letrehozas, projekt_statusz_valtas
  ...todoToolDefinitions,          // teendo_letrehozas, teendo_lekeres
  ...invoiceSchemaToolDefinitions  // szamla_lekerdezes, schema_lekerdezes
];

// ── Routing map ───────────────────────────────────────────────────────────────
const CONTACT_TOOLS = new Set(contactToolDefinitions.map(t => t.name));
const PROJECT_TOOLS = new Set(projectToolDefinitions.map(t => t.name));
const TODO_TOOLS    = new Set(todoToolDefinitions.map(t => t.name));
const INV_SCH_TOOLS = new Set(invoiceSchemaToolDefinitions.map(t => t.name));

async function routeTool(name, args) {
  if (CONTACT_TOOLS.has(name)) return handleContactTool(name, args);
  if (PROJECT_TOOLS.has(name)) return handleProjectTool(name, args);
  if (TODO_TOOLS.has(name))    return handleTodoTool(name, args);
  if (INV_SCH_TOOLS.has(name)) return handleInvoiceSchemaTool(name, args);
  throw new Error(msg.unknownTool(name));
}

// ── MCP Server setup ──────────────────────────────────────────────────────────
const server = new Server(
  { name: 'minicrm-mcp', version: '1.0.0' },
  { capabilities: { tools: {} } }
);

// LIST TOOLS
server.setRequestHandler(ListToolsRequestSchema, async () => {
  logger.debug('ListTools requested');
  return { tools: ALL_TOOLS };
});

// CALL TOOL
server.setRequestHandler(CallToolRequestSchema, async (request) => {
  const { name, arguments: args } = request.params;
  logger.info(`CallTool: ${name}`, { args });

  try {
    const text = await routeTool(name, args ?? {});
    return {
      content: [{ type: 'text', text: String(text) }]
    };
  } catch (err) {
    logger.error(`Tool error [${name}]:`, err);
    const errMsg = err instanceof Error ? err.message : String(err);
    return {
      content: [{ type: 'text', text: `${msg.toolError(name)}\n${errMsg}` }],
      isError: true
    };
  }
});

// ── Start ─────────────────────────────────────────────────────────────────────
async function main() {
  const lang = (process.env.LANGUAGE_ID || 'hu').toUpperCase();
  logger.info(`miniCRM MCP Server starting... [LANGUAGE_ID=${lang}]`);
  logger.info(`Gateway URL: ${process.env.GATEWAY_URL || 'http://localhost:5000'}`);
  logger.info(`Loaded tools (${ALL_TOOLS.length}): ${ALL_TOOLS.map(t => t.name).join(', ')}`);

  const transport = new StdioServerTransport();
  await server.connect(transport);

  logger.info('miniCRM MCP Server started. Waiting for Claude Desktop connection...');
}

main().catch(err => {
  logger.error('Fatal error during MCP server startup:', err);
  process.exit(1);
});
