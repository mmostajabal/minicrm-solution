/**
 * English strings – LANGUAGE_ID=en
 */
export default {
  // ── Contacts ──────────────────────────────────────────────────────────────
  kontakt_kereses: {
    description:
      'Search contacts in miniCRM by name, email or phone. ' +
      'Returns all matching contacts. If multiple results are found, all are returned ' +
      'so the user can choose the correct one.',
    Name:        'Search by partial name (e.g. "Smith")',
    Email:       'Search by email address',
    Phone:       'Search by phone number',
    StatusId:    'Filter by status ID',
    UserId:      'Filter by responsible user ID',
    Page:        'Page number (default: 1)',
    MaxElements: 'Maximum number of results (default: 100)',
  },
  kontakt_lekeres: {
    description:
      'Retrieve the full profile of a contact by ID. ' +
      'Returns name, email, phone, company and all custom fields.',
    Id: 'The unique contact ID',
  },
  kontakt_letrehozas: {
    description:
      'Create a new contact in miniCRM. ' +
      'IMPORTANT: always show the data to the user and ask for confirmation before executing!',
    FirstName:   'First name',
    LastName:    'Last name',
    Email:       'Email address',
    Phone:       'Phone number',
    Type:        '"Person" or "Company"',
    Description: 'Notes / description',
    Position:    'Job title / position',
  },
  kontakt_modositas: {
    description:
      'Update an existing contact by ID. ' +
      'Only the provided fields are updated. ' +
      'IMPORTANT: always show the changes to the user and ask for confirmation before executing!',
    Id:          'ID of the contact to update',
    FirstName:   'New first name',
    LastName:    'New last name',
    Email:       'New email address',
    Phone:       'New phone number',
    Description: 'New notes',
    Position:    'New job title',
  },

  // ── Projects ──────────────────────────────────────────────────────────────
  projekt_kereses: {
    description:
      'Search and filter projects / deals in miniCRM. ' +
      'Filterable by category, status, contact, responsible user and name.',
    CategoryId:  'Project category ID',
    StatusId:    'Status ID',
    ContactId:   'Related contact ID',
    UserId:      'Responsible user ID',
    Name:        'Search by partial name',
    Page:        'Page number',
    MaxElements: 'Maximum results',
  },
  projekt_lekeres: {
    description:
      'Retrieve the full details of a project: status, owner, ' +
      'related contact, custom fields and history.',
    Id: 'The unique project ID',
  },
  projekt_letrehozas: {
    description:
      'Create a new project / deal in miniCRM. ' +
      'IMPORTANT: always show the data to the user and ask for confirmation before executing!',
    CategoryId:  'Project category ID (required)',
    ContactId:   'Related contact ID',
    Name:        'Project name',
    UserId:      'Responsible user ID',
    Description: 'Description / notes',
  },
  projekt_statusz_valtas: {
    description:
      'Change the status of ONE project. ' +
      'Modifies ONLY the StatusId field – no other data is changed. ' +
      'IMPORTANT: show the new status to the user and ask for confirmation before executing!',
    Id:       'Project ID',
    StatusId: 'New status ID',
  },

  // ── Todos ─────────────────────────────────────────────────────────────────
  teendo_letrehozas: {
    description:
      'Create a new todo / task for a project. ' +
      'IMPORTANT: always show the data to the user and ask for confirmation before executing!',
    ProjectId: 'ID of the project this todo belongs to',
    UserId:    'ID of the responsible user',
    Type:      'Todo type',
    Deadline:  'Deadline in ISO 8601 format (e.g. "2026-04-20T10:00:00")',
    Comment:   'Todo description / comment',
  },
  teendo_lekeres: {
    description:
      'Retrieve the todo list for a project. ' +
      'Returns type, deadline, status and owner for each todo.',
    CardId: 'Project ID (CardId = ProjectId in the R3 API)',
  },

  // ── Invoices + Schema ─────────────────────────────────────────────────────
  szamla_lekerdezes: {
    description:
      'Query invoices for a project or contact. ' +
      'Returns amounts, statuses and due dates. ' +
      'Read-only – invoices cannot be modified via MCP.',
    ProjectId:   'Project ID',
    ContactId:   'Contact ID',
    Status:      'Filter by invoice status',
    Page:        'Page number',
    MaxElements: 'Maximum results',
  },
  schema_lekerdezes: {
    description:
      'Query the available modules, project categories, statuses and custom fields ' +
      'in this miniCRM account. Useful for resolving the correct IDs and status names.',
    Type:
      'Type to query: "categories" = all categories and statuses, ' +
      'or a specific module name (e.g. "Project", "Contact")',
  },

  // ── Response messages ─────────────────────────────────────────────────────
  messages: {
    noContact:         'No contacts found matching the given criteria.',
    contactCreated:    '✅ Contact created successfully!',
    contactUpdated:    '✅ Contact updated successfully!',
    contactsFound:     (n) => `**${n} contact(s) found:**`,
    noProject:         'No projects found matching the given criteria.',
    projectCreated:    '✅ Project created successfully!',
    statusChanged:     '✅ Project status changed successfully!',
    projectsFound:     (n) => `**${n} project(s) found:**`,
    noTodo:            'No todos found for this project.',
    todoCreated:       '✅ Todo created successfully!',
    todosFound:        (n) => `**${n} todo(s):**`,
    noInvoice:         'No invoices found matching the given criteria.',
    invoicesFound:     (n) => `**${n} invoice(s):**`,
    schemaHeader:      (type) => `**miniCRM schema – ${type}:**`,
    noData:            'No data.',
    unknownTool:       (name) => `Unknown tool: ${name}`,
    toolError:         (name) => `❌ Error running tool "${name}":`,
    rateLimitError:    'API rate limit reached (60 requests/minute). Please wait a moment.',
    authError:         'Authentication error – check your GATEWAY_API_KEY setting.',
    notFoundError:     (msg) => `Resource not found: ${msg}`,
    apiError:          (status, msg) => `API error (${status}): ${msg}`,
    statusDone:        '✅ Done',
    statusOpen:        '🔄 Open',
  },
};
