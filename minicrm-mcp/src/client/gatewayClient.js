import axios from 'axios';
import { RateLimiter } from '../utils/rateLimiter.js';
import logger from '../utils/logger.js';
import { msg } from '../i18n/index.js';

const limiter = new RateLimiter(60, 60_000);

/**
 * Configured Axios instance that talks to the miniCRM API Gateway.
 * Adds the X-Gateway-Key header to every request and self-throttles
 * to stay within the 60 req/min limit.
 */
const gateway = axios.create({
  baseURL: process.env.GATEWAY_URL || 'http://localhost:5000',
  timeout: 30_000,
  headers: {
    'Content-Type': 'application/json',
    'Accept':       'application/json',
  },
});

// Inject gateway key + rate limiting before every request
gateway.interceptors.request.use(async config => {
  await limiter.acquire();
  config.headers['X-Gateway-Key'] = process.env.GATEWAY_API_KEY || '';
  logger.debug(`→ ${config.method?.toUpperCase()} ${config.baseURL}${config.url}`);
  return config;
});

// Translate HTTP errors into localised messages
gateway.interceptors.response.use(
  res => {
    logger.debug(`← ${res.status} ${res.config.url}`);
    return res;
  },
  err => {
    const status = err.response?.status;
    const data   = err.response?.data;
    logger.error(`Gateway error ${status}: ${JSON.stringify(data)}`);

    const raw = data?.error || err.message;
    const message =
      status === 429 ? msg.rateLimitError :
      status === 401 ? msg.authError :
      status === 404 ? msg.notFoundError(raw) :
      msg.apiError(status, raw);

    const enhanced    = new Error(message);
    enhanced.status   = status;
    enhanced.data     = data;
    return Promise.reject(enhanced);
  }
);

export default gateway;
