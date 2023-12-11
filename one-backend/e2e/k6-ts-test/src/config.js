import { jUnit, textSummary } from 'https://jslib.k6.io/k6-summary/0.0.3/index.js';

/**
 * The configuration options for the application.
 * @typedef {Object} AppConfig
 * @property {string} GAME_SERVICE - The URL of the game service.
 * @property {string} AUTH_SERVICE - The URL of the authentication service.
 * @property {string} USERNAME - The username to use for authentication.
 * @property {string} PASSWORD - The password to use for authentication.
 * @property {number} MAX_SLEEP_SECONDS - The maximum number of seconds to sleep.
 * @property {K6Options} K6_OPTIONS - The k6 options.
 * @property {function} K6_SUMMARY - The k6 summary handler.
 */

/**
 * The k6 options.
 * @typedef {Object} K6Options
 * @type {{duration: string, vus: number}}
 */

const options = {
  vus: 1,
  thresholds: {
    checks: [{ threshold: 'rate == 1.00', abortOnFail: false }],
    // http_req_failed: [{ threshold: 'rate == 100.00', abortOnFail: false }],
  },
};

/**
 * Handle the end-of-test summary.
 * @param data
 * @returns {{"junit.xml": *}}
 */
const handleSummary = (data) => {
  console.log(data);
  return {
    stdout: textSummary(data),
    'junit.xml': jUnit(data), // Transform summary and save it as a JUnit XML...
  };
};

/**
 * The application configuration.
 * @type {AppConfig}
 */
export const config = {
  MAX_SLEEP_SECONDS: 10,
  GAME_SERVICE: 'http://localhost:8090',
  AUTH_SERVICE: 'http://localhost:9453',
  USERNAME: 'xrspacetest1000@xrspace.io',
  PASSWORD: '1qazXSW@',
  K6_OPTIONS: options,
  K6_SUMMARY: handleSummary,
};
