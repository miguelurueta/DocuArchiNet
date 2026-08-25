'use strict';

const { spawn } = require('node:child_process');

function requireInteractiveConsole() {
  if (!process.stdin.isTTY || !process.stdout.isTTY) {
    throw new Error('La E2E requiere una consola interactiva. No se inició ninguna prueba ni se mostraron valores.');
  }
}

function prompt(label, defaultValue) {
  return new Promise((resolve) => {
    const suffix = defaultValue === undefined ? '' : ` [${defaultValue}]`;
    process.stdout.write(`${label}${suffix}: `);
    let value = '';
    const onData = (chunk) => {
      const text = String(chunk);
      if (text.includes('\u0003')) {
        cleanup();
        resolve('');
        return;
      }
      const lineEnd = text.search(/[\r\n]/);
      if (lineEnd >= 0) {
        value += text.slice(0, lineEnd);
        cleanup();
        resolve(value.trim() || defaultValue || '');
        return;
      }
      value += text;
    };
    const cleanup = () => {
      process.stdin.off('data', onData);
      process.stdin.pause();
    };
    process.stdin.setEncoding('utf8');
    process.stdin.resume();
    process.stdin.on('data', onData);
  });
}

function promptSecret(label) {
  return new Promise((resolve) => {
    process.stdout.write(`${label}: `);
    let value = '';
    const onData = (chunk) => {
      for (const character of String(chunk)) {
        if (character === '\u0003') {
          cleanup();
          resolve('');
          return;
        }
        if (character === '\r' || character === '\n') {
          cleanup();
          process.stdout.write('\n');
          resolve(value);
          return;
        }
        if (character === '\b' || character === '\u007f') {
          if (value.length > 0) {
            value = value.slice(0, -1);
            process.stdout.write('\b \b');
          }
          continue;
        }
        value += character;
        process.stdout.write('*');
      }
    };
    const cleanup = () => {
      process.stdin.off('data', onData);
      process.stdin.setRawMode(false);
      process.stdin.pause();
    };
    process.stdin.setEncoding('utf8');
    process.stdin.setRawMode(true);
    process.stdin.resume();
    process.stdin.on('data', onData);
  });
}

async function collectValue(values, name, label, options = {}) {
  const value = options.secret ? await promptSecret(label) : await prompt(label, options.defaultValue);
  if (!value.trim()) throw new Error(`${name} es obligatoria. No se mostró ningún valor.`);
  values[name] = value.trim();
}

async function collectConfirmation(values, name, label) {
  const confirmation = await prompt(`${label} (escriba SI)`, undefined);
  if (confirmation.toUpperCase() !== 'SI') {
    throw new Error(`${name} no fue confirmada. No se inició ninguna prueba.`);
  }
  values[name] = 'true';
}

function runChild(command, args, cwd, environment, options = {}) {
  return new Promise((resolve, reject) => {
    const nonInteractiveChild = options.nonInteractiveChild === true;
    const child = spawn(command, args, {
      cwd,
      env: environment,
      stdio: nonInteractiveChild ? ['ignore', 'pipe', 'pipe'] : 'inherit',
      shell: false
    });
    if (nonInteractiveChild) {
      child.stdout?.pipe(process.stdout, { end: false });
      child.stderr?.pipe(process.stderr, { end: false });
    }
    child.once('error', reject);
    child.once(nonInteractiveChild ? 'close' : 'exit', (code, signal) => resolve({ code: code ?? 1, signal }));
  });
}

module.exports = {
  collectConfirmation,
  collectValue,
  promptSecret,
  requireInteractiveConsole,
  runChild
};
