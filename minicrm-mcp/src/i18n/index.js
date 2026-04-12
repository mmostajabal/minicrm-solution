/**
 * i18n loader – picks the right language bundle based on LANGUAGE_ID env var.
 *
 * Supported values:  hu  (Hungarian / Magyar)   ← default
 *                    en  (English)
 *
 * Set in .env:  LANGUAGE_ID=en
 * Or in claude_desktop_config.json env block.
 */

import hu from './hu.js';
import en from './en.js';

const SUPPORTED = { hu, en };

const id = (process.env.LANGUAGE_ID || 'hu').toLowerCase().trim();

if (!SUPPORTED[id]) {
  process.stderr.write(
    `[i18n] Unknown LANGUAGE_ID="${id}". Falling back to "hu". Supported: ${Object.keys(SUPPORTED).join(', ')}\n`
  );
}

const strings = SUPPORTED[id] ?? hu;

export const msg = strings.messages;

/**
 * Returns the description and property labels for a named tool.
 * @param {string} toolName
 */
export function t(toolName) {
  return strings[toolName] ?? {};
}

export default strings;
