/**
 * Client-side sliding-window rate limiter.
 * Mirrors the server-side limit so the MCP server self-throttles
 * before hitting a 429 from the API Gateway / miniCRM.
 */
export class RateLimiter {
  /** @param {number} maxRequests @param {number} windowMs */
  constructor(maxRequests = 60, windowMs = 60_000) {
    this.maxRequests = maxRequests;
    this.windowMs   = windowMs;
    /** @type {number[]} timestamps of recent calls */
    this.timestamps = [];
  }

  /**
   * Wait until a slot is available, then record the call.
   * @param {number} [timeoutMs=10000] how long to wait before throwing
   */
  async acquire(timeoutMs = 10_000) {
    const deadline = Date.now() + timeoutMs;
    while (true) {
      this._purge();
      if (this.timestamps.length < this.maxRequests) {
        this.timestamps.push(Date.now());
        return;
      }
      if (Date.now() >= deadline) {
        throw new Error('Rate limit exceeded – max 60 requests/minute.');
      }
      // Wait until the oldest entry expires
      const oldest  = this.timestamps[0];
      const waitMs  = Math.min(oldest + this.windowMs - Date.now(), 1000);
      await sleep(Math.max(waitMs, 100));
    }
  }

  _purge() {
    const cutoff = Date.now() - this.windowMs;
    this.timestamps = this.timestamps.filter(t => t > cutoff);
  }

  get currentCount() {
    this._purge();
    return this.timestamps.length;
  }
}

function sleep(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
}
