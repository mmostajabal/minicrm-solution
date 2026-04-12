import winston from 'winston';

const { combine, timestamp, printf, colorize, errors } = winston.format;

const logFormat = printf(({ level, message, timestamp, stack }) =>
  `[${timestamp}] ${level}: ${stack || message}`
);

const transports = [
  // MCP uses stdio – log to stderr so it does not pollute the MCP protocol stream
  new winston.transports.Console({
    stderrLevels: ['error', 'warn', 'info', 'debug', 'verbose', 'silly'],
    format: combine(
      colorize(),
      timestamp({ format: 'HH:mm:ss' }),
      errors({ stack: true }),
      logFormat
    )
  })
];

if (process.env.LOG_FILE) {
  transports.push(
    new winston.transports.File({
      filename: process.env.LOG_FILE,
      format: combine(
        timestamp(),
        errors({ stack: true }),
        winston.format.json()
      )
    })
  );
}

const logger = winston.createLogger({
  level: process.env.LOG_LEVEL || 'info',
  transports
});

export default logger;
