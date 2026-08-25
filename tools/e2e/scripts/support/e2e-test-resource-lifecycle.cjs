'use strict';

const crypto = require('node:crypto');
const fs = require('node:fs/promises');
const path = require('node:path');

const RESOURCE_CODES = Object.freeze({
  CONTRACT_INVALID: 'E2E_RESOURCE_CONTRACT_INVALID',
  DESCRIPTOR_INVALID: 'E2E_RESOURCE_DESCRIPTOR_INVALID',
  PREFLIGHT_INVALID: 'E2E_RESOURCE_PREFLIGHT_INVALID',
  UNAVAILABLE: 'E2E_RESOURCE_UNAVAILABLE',
  RESERVED: 'E2E_RESOURCE_RESERVED',
  CONSUMED: 'E2E_RESOURCE_CONSUMED',
  RESERVATION_IN_PROGRESS: 'E2E_RESOURCE_RESERVATION_IN_PROGRESS',
  STATE_INVALID: 'E2E_RESOURCE_STATE_INVALID',
  SHARED_COORDINATOR_REQUIRED: 'E2E_RESOURCE_SHARED_COORDINATOR_REQUIRED'
});

const sensitiveKey = /(passw(?:ord)?|pwd|cookie|token|secret|credential|credencial|connection|conexion|authorization|authorized|comando|command|script|provider|proveedor|path|ruta)/i;
const forbiddenValue = /(?:^|[;\s])(?:password|pwd|uid)\s*=|\b(?:SELECT|INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b|(?:mysql|odbc):\/\//i;
const safeIdentifier = /^[a-z][a-z0-9-]{1,79}$/;
const safeRole = /^[a-z][a-z0-9-]{1,79}$/;
const safeCode = /^E2E_RESOURCE_[A-Z0-9_]{3,100}$/;

class ResourceLifecycleError extends Error {
  constructor(code) {
    super(`El recurso E2E no está disponible para esta etapa (${code}).`);
    this.name = 'ResourceLifecycleError';
    this.code = code;
  }
}

function fail(code) {
  throw new ResourceLifecycleError(code);
}

function opaqueHash(value) {
  return crypto.createHash('sha256').update(String(value), 'utf8').digest('hex');
}

function assertSafeIdentifier(value, code = RESOURCE_CODES.CONTRACT_INVALID) {
  if (typeof value !== 'string' || !safeIdentifier.test(value)) fail(code);
}

function assertSafeRole(value, code = RESOURCE_CODES.CONTRACT_INVALID) {
  if (typeof value !== 'string' || !safeRole.test(value)) fail(code);
}

function assertSafeCode(value, fallback = RESOURCE_CODES.PREFLIGHT_INVALID) {
  if (typeof value !== 'string' || !safeCode.test(value)) return fallback;
  return value;
}

function assertNonSensitiveDescriptor(value, depth = 0) {
  if (depth > 8) fail(RESOURCE_CODES.DESCRIPTOR_INVALID);
  if (value === null || typeof value === 'boolean' || typeof value === 'number') return;
  if (typeof value === 'string') {
    if (value.length > 512 || forbiddenValue.test(value)) fail(RESOURCE_CODES.DESCRIPTOR_INVALID);
    return;
  }
  if (Array.isArray(value)) {
    if (value.length > 50) fail(RESOURCE_CODES.DESCRIPTOR_INVALID);
    for (const entry of value) assertNonSensitiveDescriptor(entry, depth + 1);
    return;
  }
  if (!value || typeof value !== 'object') fail(RESOURCE_CODES.DESCRIPTOR_INVALID);
  const keys = Object.keys(value);
  if (keys.length > 30) fail(RESOURCE_CODES.DESCRIPTOR_INVALID);
  for (const key of keys) {
    if (sensitiveKey.test(key)) fail(RESOURCE_CODES.DESCRIPTOR_INVALID);
    assertNonSensitiveDescriptor(value[key], depth + 1);
  }
}

function validateResourceContract(contract) {
  if (!contract || typeof contract !== 'object' || Array.isArray(contract)) fail(RESOURCE_CODES.CONTRACT_INVALID);
  assertSafeIdentifier(contract.id);
  if (contract.scope !== 'local' && contract.scope !== 'shared') fail(RESOURCE_CODES.CONTRACT_INVALID);
  if (!contract.resources || typeof contract.resources !== 'object' || Array.isArray(contract.resources)) fail(RESOURCE_CODES.CONTRACT_INVALID);
  const roles = Object.entries(contract.resources);
  if (roles.length === 0 || roles.length > 20) fail(RESOURCE_CODES.CONTRACT_INVALID);
  for (const [role, resource] of roles) {
    assertSafeRole(role);
    if (!resource || typeof resource !== 'object' || Array.isArray(resource) ||
        typeof resource.descriptor !== 'function' || typeof resource.preflight !== 'function' ||
        typeof resource.observeGeneration !== 'function' || typeof resource.consumeOnSuccess !== 'boolean') {
      fail(RESOURCE_CODES.CONTRACT_INVALID);
    }
  }
  return contract;
}

function validateRegisteredResourceContracts(contracts) {
  if (!contracts || typeof contracts !== 'object' || Array.isArray(contracts)) fail(RESOURCE_CODES.CONTRACT_INVALID);
  const entries = Object.entries(contracts);
  if (entries.length === 0) fail(RESOURCE_CODES.CONTRACT_INVALID);
  for (const [name, contract] of entries) {
    assertSafeIdentifier(name);
    validateResourceContract(contract);
    if (name !== contract.id) fail(RESOURCE_CODES.CONTRACT_INVALID);
  }
  return contracts;
}

function resourceIdentity(contractId, scope, role, resourceKey) {
  if (typeof resourceKey !== 'string' || !resourceKey.trim() || resourceKey.length > 1024) fail(RESOURCE_CODES.PREFLIGHT_INVALID);
  return opaqueHash(`${contractId}\u0000${scope}\u0000${role}\u0000${resourceKey}`);
}

function generationHash(generation) {
  if (typeof generation !== 'string' || !generation.trim() || generation.length > 4096) fail(RESOURCE_CODES.PREFLIGHT_INVALID);
  return opaqueHash(generation);
}

function defaultLeaseRoot() {
  return path.resolve(__dirname, '..', '..', 'artifacts', 'resource-leases');
}

function stateFile(root, identity) {
  return path.join(root, `${identity}.json`);
}

function guardDirectory(root, identity) {
  return path.join(root, `${identity}.guard`);
}

async function readState(file) {
  try {
    const parsed = JSON.parse(await fs.readFile(file, 'utf8'));
    if (!parsed || typeof parsed !== 'object' ||
        (parsed.status !== 'leased' && parsed.status !== 'consumed') ||
        typeof parsed.token !== 'string' || !/^[a-f0-9-]{36}$/i.test(parsed.token) ||
        typeof parsed.generationHash !== 'string' || !/^[a-f0-9]{64}$/i.test(parsed.generationHash)) {
      fail(RESOURCE_CODES.STATE_INVALID);
    }
    return parsed;
  } catch (error) {
    if (error?.code === 'ENOENT') return null;
    if (error instanceof ResourceLifecycleError) throw error;
    fail(RESOURCE_CODES.STATE_INVALID);
  }
}

async function writeState(file, state) {
  await fs.writeFile(file, `${JSON.stringify(state)}\n`, 'utf8');
}

function createLocalLeaseStore({ root = defaultLeaseRoot() } = {}) {
  const resolvedRoot = path.resolve(root);
  return {
    scope: 'local',
    root: resolvedRoot,
    async withGuard(identity, action) {
      await fs.mkdir(resolvedRoot, { recursive: true });
      const guard = guardDirectory(resolvedRoot, identity);
      try {
        await fs.mkdir(guard);
      } catch (error) {
        if (error?.code === 'EEXIST') fail(RESOURCE_CODES.RESERVATION_IN_PROGRESS);
        throw error;
      }
      try {
        return await action(stateFile(resolvedRoot, identity));
      } finally {
        await fs.rm(guard, { recursive: true, force: true });
      }
    },
    async acquire({ contractId, scope, role, resourceKey, generation }) {
      if (scope !== 'local') fail(RESOURCE_CODES.SHARED_COORDINATOR_REQUIRED);
      const identity = resourceIdentity(contractId, scope, role, resourceKey);
      const observedGenerationHash = generationHash(generation);
      return this.withGuard(identity, async (file) => {
        const existing = await readState(file);
        if (existing?.status === 'leased') fail(RESOURCE_CODES.RESERVED);
        if (existing?.status === 'consumed' && existing.generationHash === observedGenerationHash) fail(RESOURCE_CODES.CONSUMED);
        const token = crypto.randomUUID();
        await writeState(file, {
          version: 1,
          status: 'leased',
          token,
          generationHash: observedGenerationHash,
          updatedAt: new Date().toISOString()
        });
        return Object.freeze({ identity, token, contractId, scope, role });
      });
    },
    async consume(lease, generation) {
      const observedGenerationHash = generationHash(generation);
      return this.withGuard(lease.identity, async (file) => {
        const existing = await readState(file);
        if (!existing || existing.status !== 'leased' || existing.token !== lease.token) fail(RESOURCE_CODES.STATE_INVALID);
        await writeState(file, {
          version: 1,
          status: 'consumed',
          token: lease.token,
          generationHash: observedGenerationHash,
          updatedAt: new Date().toISOString()
        });
        return Object.freeze({ code: 'E2E_RESOURCE_CONSUMED', generationHash: observedGenerationHash });
      });
    },
    async release(lease) {
      return this.withGuard(lease.identity, async (file) => {
        const existing = await readState(file);
        if (!existing || existing.status !== 'leased' || existing.token !== lease.token) fail(RESOURCE_CODES.STATE_INVALID);
        await fs.rm(file, { force: true });
        return Object.freeze({ code: 'E2E_RESOURCE_RELEASED' });
      });
    }
  };
}

function validatePreflight(result) {
  if (!result || typeof result !== 'object' || Array.isArray(result) || typeof result.available !== 'boolean') {
    fail(RESOURCE_CODES.PREFLIGHT_INVALID);
  }
  const code = assertSafeCode(result.code, result.available ? 'E2E_RESOURCE_READY' : RESOURCE_CODES.UNAVAILABLE);
  if (!result.available) return Object.freeze({ available: false, code });
  if (typeof result.resourceKey !== 'string' || typeof result.generation !== 'string') fail(RESOURCE_CODES.PREFLIGHT_INVALID);
  return Object.freeze({ available: true, code, resourceKey: result.resourceKey, generation: result.generation });
}

function createResourceLifecycle({ contract, profile, environment, leaseStore = createLocalLeaseStore() }) {
  validateResourceContract(contract);
  if (!leaseStore || typeof leaseStore.acquire !== 'function' || typeof leaseStore.consume !== 'function' || typeof leaseStore.release !== 'function') {
    fail(RESOURCE_CODES.CONTRACT_INVALID);
  }
  const events = [];
  const active = new Set();

  function evidence() {
    return events.map((event) => ({ ...event }));
  }

  async function prepare(role) {
    assertSafeRole(role);
    const resource = contract.resources[role];
    if (!resource) fail(RESOURCE_CODES.CONTRACT_INVALID);
    if (contract.scope === 'shared' && leaseStore.scope !== 'shared') fail(RESOURCE_CODES.SHARED_COORDINATOR_REQUIRED);
    let descriptor;
    try {
      descriptor = resource.descriptor(profile);
      assertNonSensitiveDescriptor(descriptor);
    } catch (error) {
      if (error instanceof ResourceLifecycleError) throw error;
      fail(RESOURCE_CODES.DESCRIPTOR_INVALID);
    }
    let preflight;
    try {
      preflight = validatePreflight(await resource.preflight({ profile, environment, role, descriptor }));
    } catch (error) {
      if (error instanceof ResourceLifecycleError) throw error;
      fail(RESOURCE_CODES.UNAVAILABLE);
    }
    events.push(Object.freeze({ role, phase: 'preflight', code: preflight.code }));
    if (!preflight.available) fail(preflight.code);
    const lease = await leaseStore.acquire({
      contractId: contract.id,
      scope: contract.scope,
      role,
      resourceKey: preflight.resourceKey,
      generation: preflight.generation
    });
    const reservation = Object.freeze({ role, resource, descriptor, lease });
    active.add(reservation);
    events.push(Object.freeze({ role, phase: 'reserved', code: 'E2E_RESOURCE_RESERVED' }));
    return reservation;
  }

  async function finalize(reservation, successful) {
    if (!active.has(reservation)) return;
    active.delete(reservation);
    if (successful && reservation.resource.consumeOnSuccess) {
      let generation;
      try {
        generation = await reservation.resource.observeGeneration({
          profile,
          environment,
          role: reservation.role,
          descriptor: reservation.descriptor
        });
      } catch {
        fail(RESOURCE_CODES.UNAVAILABLE);
      }
      await leaseStore.consume(reservation.lease, generation);
      events.push(Object.freeze({ role: reservation.role, phase: 'consumed', code: 'E2E_RESOURCE_CONSUMED' }));
      return;
    }
    await leaseStore.release(reservation.lease);
    events.push(Object.freeze({ role: reservation.role, phase: 'released', code: 'E2E_RESOURCE_RELEASED' }));
  }

  async function close() {
    const reservations = [...active].reverse();
    for (const reservation of reservations) await finalize(reservation, false);
  }

  return Object.freeze({ prepare, finalize, close, evidence });
}

async function writeResourceLifecycleEvidence({ root, doc, contractId, events }) {
  assertSafeIdentifier(doc, RESOURCE_CODES.CONTRACT_INVALID);
  assertSafeIdentifier(contractId, RESOURCE_CODES.CONTRACT_INVALID);
  if (!Array.isArray(events)) fail(RESOURCE_CODES.STATE_INVALID);
  const evidence = { fechaUtc: new Date().toISOString(), doc, contractId, resources: events };
  const serialized = JSON.stringify(evidence);
  if (/passw(?:ord)?|pwd|cookie|token|secret|credential|credencial|connection|conexion|authorization|authorized|workflow-task:\d+/i.test(serialized)) {
    fail(RESOURCE_CODES.STATE_INVALID);
  }
  const destination = path.resolve(root, 'tools', 'e2e', 'artifacts', `resource-lifecycle-${contractId}.json`);
  if (path.relative(root, destination).startsWith('..')) fail(RESOURCE_CODES.STATE_INVALID);
  await fs.mkdir(path.dirname(destination), { recursive: true });
  await fs.writeFile(destination, `${JSON.stringify(evidence, null, 2)}\n`, 'utf8');
}

module.exports = {
  RESOURCE_CODES,
  ResourceLifecycleError,
  assertNonSensitiveDescriptor,
  createLocalLeaseStore,
  createResourceLifecycle,
  defaultLeaseRoot,
  opaqueHash,
  validateRegisteredResourceContracts,
  validateResourceContract,
  writeResourceLifecycleEvidence
};
