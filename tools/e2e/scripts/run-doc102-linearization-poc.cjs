'use strict';

const crypto = require('node:crypto');
const fs = require('node:fs');
const http = require('node:http');
const os = require('node:os');
const path = require('node:path');
const { spawnSync } = require('node:child_process');
const { chromium } = require('@playwright/test');

const repoRoot = path.resolve(__dirname, '../../..');
const fixture = path.join(repoRoot, 'help', 'help_searh_public_document.pdf');
const pdfJs = path.join(repoRoot, 'pdfjs', 'build', 'pdf.js');
const pdfWorker = path.join(repoRoot, 'pdfjs', 'build', 'pdf.worker.js');
const rangeChunkSize = 262144;
const fixtureRepetitions = 8;
const transportChunkBytes = 65536;
const transportChunkDelayMs = 20;

function sha256(file) {
  return crypto.createHash('sha256').update(fs.readFileSync(file)).digest('hex');
}

function findGhostscript() {
  const lookup = spawnSync('where.exe', ['gswin64c'], {
    encoding: 'utf8',
    windowsHide: true
  });
  if (lookup.status !== 0) {
    throw new Error('DOC102_GHOSTSCRIPT_NOT_FOUND');
  }
  const executable = lookup.stdout.split(/\r?\n/).map(value => value.trim()).find(Boolean);
  if (!executable) {
    throw new Error('DOC102_GHOSTSCRIPT_NOT_FOUND');
  }
  return executable;
}

function runGhostscript(executable, args, errorCode) {
  const execution = spawnSync(executable, args, {
    encoding: 'utf8',
    maxBuffer: 64 * 1024 * 1024,
    windowsHide: true
  });
  if (execution.status !== 0) {
    throw new Error(errorCode);
  }
}

function isLinearized(file) {
  const handle = fs.openSync(file, 'r');
  try {
    const buffer = Buffer.alloc(4096);
    const bytesRead = fs.readSync(handle, buffer, 0, buffer.length, 0);
    return buffer.subarray(0, bytesRead).toString('latin1').includes('/Linearized');
  } finally {
    fs.closeSync(handle);
  }
}

function parseRange(value, length) {
  const match = /^bytes=(\d+)-(\d*)$/.exec(value || '');
  if (!match) return null;
  const start = Number(match[1]);
  const requestedEnd = match[2] ? Number(match[2]) : length - 1;
  if (!Number.isSafeInteger(start) || !Number.isSafeInteger(requestedEnd) ||
      start < 0 || start >= length || requestedEnd < start) return null;
  return { start, end: Math.min(requestedEnd, length - 1) };
}

function viewerHtml(variant) {
  return `<!doctype html>
<html><body><canvas id="page"></canvas><script src="/pdf.js"></script><script>
pdfjsLib.GlobalWorkerOptions.workerSrc = '/pdf.worker.js';
(async () => {
  const started = performance.now();
  const loadingTask = pdfjsLib.getDocument({
    url: '/${variant}.pdf',
    rangeChunkSize: ${rangeChunkSize},
    disableStream: true,
    disableAutoFetch: true
  });
  try {
    const documentRef = await loadingTask.promise;
    const page = await documentRef.getPage(1);
    const viewport = page.getViewport({ scale: 1 });
    const canvas = document.getElementById('page');
    canvas.width = Math.ceil(viewport.width);
    canvas.height = Math.ceil(viewport.height);
    await page.render({ canvasContext: canvas.getContext('2d'), viewport }).promise;
    window.__doc102Result = {
      pageCount: documentRef.numPages,
      firstCanvasMs: Math.round(performance.now() - started),
      canvas: canvas.toDataURL('image/png')
    };
  } catch (error) {
    window.__doc102Error = error && error.name ? error.name : 'PDF_RENDER_FAILED';
  }
})();
</script></body></html>`;
}

function startServer(files, counters) {
  function streamCounted(file, options, response, counter, initialOffset) {
    const stream = fs.createReadStream(file, { ...options, highWaterMark: transportChunkBytes });
    let closed = false;
    let offset = initialOffset;
    response.once('close', () => {
      closed = true;
      stream.destroy();
    });
    stream.on('data', chunk => {
      if (closed) return;
      counter.bytes += chunk.length;
      counter.intervals.push([offset, offset + chunk.length - 1]);
      offset += chunk.length;
      stream.pause();
      response.write(chunk);
      setTimeout(() => {
        if (!closed) stream.resume();
      }, transportChunkDelayMs);
    });
    stream.once('end', () => {
      if (!closed) response.end();
    });
    stream.once('error', () => {
      if (!closed) response.destroy();
    });
  }

  const server = http.createServer((request, response) => {
    const requestUrl = new URL(request.url, 'http://127.0.0.1');
    if (requestUrl.pathname === '/pdf.js' || requestUrl.pathname === '/pdf.worker.js') {
      const asset = requestUrl.pathname === '/pdf.js' ? pdfJs : pdfWorker;
      const stat = fs.statSync(asset);
      response.writeHead(200, {
        'Cache-Control': 'no-store',
        'Content-Length': stat.size,
        'Content-Type': 'application/javascript'
      });
      fs.createReadStream(asset).pipe(response);
      return;
    }

    const pdfMatch = /^\/(baseline|linearized)\.pdf$/.exec(requestUrl.pathname);
    if (pdfMatch) {
      const variant = pdfMatch[1];
      const file = files[variant];
      const length = fs.statSync(file).size;
      const range = parseRange(request.headers.range, length);
      counters[variant].requests += 1;
      if (range) {
        const bytes = range.end - range.start + 1;
        counters[variant].rangeRequests += 1;
        response.writeHead(206, {
          'Accept-Ranges': 'bytes',
          'Cache-Control': 'no-store',
          'Content-Length': bytes,
          'Content-Range': `bytes ${range.start}-${range.end}/${length}`,
          'Content-Type': 'application/pdf'
        });
        streamCounted(file, { start: range.start, end: range.end }, response, counters[variant], range.start);
        return;
      }
      response.writeHead(200, {
        'Accept-Ranges': 'bytes',
        'Cache-Control': 'no-store',
        'Content-Length': length,
        'Content-Type': 'application/pdf'
      });
      streamCounted(file, {}, response, counters[variant], 0);
      return;
    }

    const variant = requestUrl.searchParams.get('variant');
    if (requestUrl.pathname === '/viewer' && (variant === 'baseline' || variant === 'linearized')) {
      const body = Buffer.from(viewerHtml(variant));
      response.writeHead(200, {
        'Cache-Control': 'no-store',
        'Content-Length': body.length,
        'Content-Type': 'text/html; charset=utf-8'
      });
      response.end(body);
      return;
    }

    response.writeHead(404, { 'Content-Length': 0 });
    response.end();
  });

  return new Promise((resolve, reject) => {
    server.once('error', reject);
    server.listen(0, '127.0.0.1', () => resolve(server));
  });
}

async function measure(browser, origin, variant, counters, fileBytes) {
  const context = await browser.newContext();
  const page = await context.newPage();
  try {
    await page.goto(`${origin}/viewer?variant=${variant}`, { waitUntil: 'domcontentloaded' });
    await page.waitForFunction(() => window.__doc102Result || window.__doc102Error, null, {
      timeout: 30000
    });
    const failure = await page.evaluate(() => window.__doc102Error || null);
    if (failure) throw new Error(`DOC102_${variant.toUpperCase()}_${failure}`);
    const browserResult = await page.evaluate(() => window.__doc102Result);
    const canvasHash = crypto.createHash('sha256').update(browserResult.canvas).digest('hex');
    const intervals = counters[variant].intervals
      .slice()
      .sort((left, right) => left[0] - right[0]);
    let uniqueBytes = 0;
    let currentStart = -1;
    let currentEnd = -1;
    for (const [start, end] of intervals) {
      if (start > currentEnd + 1) {
        if (currentStart >= 0) uniqueBytes += currentEnd - currentStart + 1;
        currentStart = start;
        currentEnd = end;
      } else {
        currentEnd = Math.max(currentEnd, end);
      }
    }
    if (currentStart >= 0) uniqueBytes += currentEnd - currentStart + 1;
    return {
      fileBytes,
      firstCanvasWireBytes: counters[variant].bytes,
      firstCanvasUniqueBytes: uniqueBytes,
      firstCanvasPercent: Number(((uniqueBytes / fileBytes) * 100).toFixed(2)),
      requests: counters[variant].requests,
      rangeRequests: counters[variant].rangeRequests,
      firstCanvasMs: browserResult.firstCanvasMs,
      pageCount: browserResult.pageCount,
      canvasHash
    };
  } finally {
    await context.close();
  }
}

async function closeServer(server) {
  if (!server) return;
  await new Promise(resolve => server.close(resolve));
}

async function main() {
  if (!fs.existsSync(fixture) || !fs.existsSync(pdfJs) || !fs.existsSync(pdfWorker)) {
    throw new Error('DOC102_POC_INPUT_MISSING');
  }

  const fixtureHashBefore = sha256(fixture);
  const fixtureMtimeBefore = fs.statSync(fixture).mtimeMs;
  const tempDirectory = fs.mkdtempSync(path.join(os.tmpdir(), 'doc102-linearization-poc-'));
  const baseline = path.join(tempDirectory, 'baseline.pdf');
  const linearized = path.join(tempDirectory, 'linearized.pdf');
  let browser;
  let server;
  let result;

  try {
    const ghostscript = findGhostscript();
    const inputs = Array.from({ length: fixtureRepetitions }, () => fixture);
    runGhostscript(ghostscript, [
      '-q', '-dBATCH', '-dNOPAUSE', '-sDEVICE=pdfwrite',
      `-sOutputFile=${baseline}`,
      ...inputs
    ], 'DOC102_BASELINE_CREATION_FAILED');
    runGhostscript(ghostscript, [
      '-q', '-dBATCH', '-dNOPAUSE', '-sDEVICE=pdfwrite', '-dFastWebView=true',
      `-sOutputFile=${linearized}`,
      baseline
    ], 'DOC102_LINEARIZATION_FAILED');

    const baselineBytes = fs.statSync(baseline).size;
    const linearizedBytes = fs.statSync(linearized).size;
    if (baselineBytes < 1048576 || isLinearized(baseline) || !isLinearized(linearized)) {
      throw new Error('DOC102_LINEARIZATION_PRECONDITION_FAILED');
    }

    const counters = {
      baseline: { requests: 0, rangeRequests: 0, bytes: 0, intervals: [] },
      linearized: { requests: 0, rangeRequests: 0, bytes: 0, intervals: [] }
    };
    server = await startServer({ baseline, linearized }, counters);
    const address = server.address();
    const origin = `http://127.0.0.1:${address.port}`;
    browser = await chromium.launch({ headless: true });
    const baselineResult = await measure(browser, origin, 'baseline', counters, baselineBytes);
    const linearizedResult = await measure(browser, origin, 'linearized', counters, linearizedBytes);
    const fixtureUnchanged = fixtureHashBefore === sha256(fixture) &&
      fixtureMtimeBefore === fs.statSync(fixture).mtimeMs;
    const equivalent = baselineResult.pageCount === linearizedResult.pageCount &&
      baselineResult.canvasHash === linearizedResult.canvasHash;
    const partial = linearizedResult.firstCanvasUniqueBytes < linearizedResult.fileBytes;

    result = {
      fixture: 'help/help_searh_public_document.pdf',
      fixtureUnchanged,
      baseline: { ...baselineResult, linearized: false },
      linearized: { ...linearizedResult, linearized: true },
      equivalent,
      partial,
      networkProfile: {
        approximateMbps: Number(((transportChunkBytes * 8) / transportChunkDelayMs / 1000).toFixed(2)),
        chunkBytes: transportChunkBytes,
        chunkDelayMs: transportChunkDelayMs
      },
      status: fixtureUnchanged && equivalent && partial ? 'PASSED' : 'FAILED'
    };
  } finally {
    if (browser) await browser.close();
    await closeServer(server);
    fs.rmSync(tempDirectory, { recursive: true, force: true });
  }

  const sanitized = {
    ...result,
    baseline: { ...result.baseline, canvasHash: undefined },
    linearized: { ...result.linearized, canvasHash: undefined },
    tempRemoved: !fs.existsSync(tempDirectory)
  };
  console.log(JSON.stringify(sanitized, null, 2));
  console.log(`DOC102_LINEARIZATION_RESULT=${sanitized.status}`);
  console.log(`DOC102_LINEARIZATION_FIRST_CANVAS_UNIQUE_BYTES=${sanitized.linearized.firstCanvasUniqueBytes}`);
  console.log(`DOC102_LINEARIZATION_FIRST_CANVAS_PERCENT=${sanitized.linearized.firstCanvasPercent}`);
  console.log(`DOC102_LINEARIZATION_TEMP_REMOVED=${sanitized.tempRemoved}`);
  if (sanitized.status !== 'PASSED' || !sanitized.tempRemoved) process.exitCode = 2;
}

main().catch(error => {
  console.error(error && error.message ? error.message : 'DOC102_LINEARIZATION_POC_FAILED');
  process.exitCode = 2;
});
