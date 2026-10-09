'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');
const mysql = require('mysql2/promise');

const authorized = process.env.DOC92_MYSQL_AUTHORIZED === 'SI';
const root = path.resolve(__dirname, '..', '..', '..');
const sqlRoot = path.join(root, 'Doc', 'Actualizacion', 'Login', 'Implementacion', 'DOC-92', 'Sql');

function options() {
  const database = process.env.DOC92_MYSQL_DATABASE || '';
  assert.match(database, /^doc92_[a-z0-9_]+$/i, 'La base descartable debe iniciar por doc92_.');
  return {
    host: process.env.DOC92_MYSQL_HOST,
    port: Number(process.env.DOC92_MYSQL_PORT || 3306),
    user: process.env.DOC92_MYSQL_USER,
    password: process.env.DOC92_MYSQL_PASSWORD,
    database,
    multipleStatements: false
  };
}

function parseClientScript(contents) {
  let delimiter = ';';
  let buffer = '';
  const statements = [];
  for (const line of contents.split(/\r?\n/)) {
    const directive = line.trim().match(/^DELIMITER\s+(.+)$/i);
    if (directive) { delimiter = directive[1]; continue; }
    buffer += `${line}\n`;
    if (buffer.trimEnd().endsWith(delimiter)) {
      const trimmed = buffer.trimEnd().slice(0, -delimiter.length).trim();
      if (trimmed) statements.push(trimmed);
      buffer = '';
    }
  }
  if (buffer.trim()) statements.push(buffer.trim());
  return statements;
}

async function runScript(connection, name) {
  const contents = fs.readFileSync(path.join(sqlRoot, name), 'utf8');
  for (const statement of parseClientScript(contents)) await connection.query(statement);
}

async function contender(config, challengeId) {
  const connection = await mysql.createConnection(config);
  try {
    await connection.beginTransaction();
    const [rows] = await connection.execute('SELECT State FROM ra_auth_second_factor_challenge WHERE ChallengeId=? FOR UPDATE', [challengeId]);
    let affected = 0;
    if (rows.length === 1 && rows[0].State === 'SENT') {
      const [result] = await connection.execute("UPDATE ra_auth_second_factor_challenge SET State='FINALIZING',UpdatedAtUtc=UTC_TIMESTAMP() WHERE ChallengeId=? AND State='SENT' AND Attempts=0 AND Consumed=0", [challengeId]);
      affected = result.affectedRows;
    }
    await connection.commit();
    return affected;
  } catch (error) {
    await connection.rollback();
    throw error;
  } finally {
    await connection.end();
  }
}

test('DOC-92: migración y concurrencia en MySQL descartable', { skip: !authorized }, async () => {
  const config = options();
  const connection = await mysql.createConnection(config);
  const challengeId = '11111111-2222-3333-4444-555555555555';
  try {
    const [[selected]] = await connection.query('SELECT DATABASE() AS db');
    assert.equal(selected.db, config.database);
    await connection.query('DROP TABLE IF EXISTS ra_auth_second_factor_challenge');
    await connection.query(`CREATE TABLE ra_auth_second_factor_challenge (
      Id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
      ChallengeId CHAR(36) NOT NULL,
      AuthUserId VARCHAR(100) NOT NULL,
      Provider VARCHAR(20) NOT NULL,
      CodeHash VARCHAR(200) NULL,
      ExpiresAtUtc DATETIME NOT NULL,
      Consumed TINYINT(1) NOT NULL DEFAULT 0,
      Attempts INT NOT NULL DEFAULT 0,
      CreatedAtUtc TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
      AuthPayloadJson LONGTEXT NULL,
      PRIMARY KEY (Id), UNIQUE KEY uq_challengeid (ChallengeId), KEY IX_ra_auth_sfc_authuserid (AuthUserId)
    ) ENGINE=InnoDB`);

    await connection.execute("INSERT INTO ra_auth_second_factor_challenge (ChallengeId,AuthUserId,Provider,CodeHash,ExpiresAtUtc,Consumed,Attempts,AuthPayloadJson) VALUES (UUID(),'legacy','EMAIL',NULL,UTC_TIMESTAMP()+INTERVAL 5 MINUTE,0,0,'legacy-payload')");
    await runScript(connection, '00-preflight.sql');
    await runScript(connection, '01-apply.sql');
    await runScript(connection, '01-apply.sql');
    await runScript(connection, '02-postflight.sql');

    const [[legacy]] = await connection.query("SELECT SchemaVersion,Purpose,State,AuthPayloadJson FROM ra_auth_second_factor_challenge WHERE AuthUserId='legacy'");
    assert.equal(legacy.SchemaVersion, null);
    assert.equal(legacy.Purpose, null);
    assert.equal(legacy.State, null);
    assert.equal(legacy.AuthPayloadJson, 'legacy-payload');

    await connection.execute(`INSERT INTO ra_auth_second_factor_challenge
      (ChallengeId,AuthUserId,Provider,Purpose,SessionBindingHash,State,KeyId,CodeHash,ExpiresAtUtc,Consumed,Attempts,ResendCount,CreatedAtUtc,UpdatedAtUtc,SchemaVersion,AuthPayloadJson)
      VALUES (?,'7:11:RADICADOR:42','EMAIL','LOGIN','session-hash','SENT','active','protected',UTC_TIMESTAMP()+INTERVAL 5 MINUTE,0,0,0,UTC_TIMESTAMP(),UTC_TIMESTAMP(),1,NULL)`, [challengeId]);
    const winners = await Promise.all([contender(config, challengeId), contender(config, challengeId)]);
    assert.equal(winners.reduce((sum, value) => sum + value, 0), 1, 'Exactamente una transición debe ganar.');

    await runScript(connection, '03-rollback.sql');
    const [[remaining]] = await connection.query("SELECT COUNT(*) AS count FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND COLUMN_NAME IN ('Purpose','SessionBindingHash','State','KeyId','ResendCount','LastSentAtUtc','TerminalAtUtc','UpdatedAtUtc','SchemaVersion')");
    assert.equal(Number(remaining.count), 0);
  } finally {
    await connection.query('DROP TABLE IF EXISTS ra_auth_second_factor_challenge');
    await connection.end();
  }
});
