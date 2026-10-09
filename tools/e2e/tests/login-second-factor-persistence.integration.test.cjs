'use strict';

const assert = require('node:assert/strict');
const test = require('node:test');
const mysql = require('mysql2/promise');

const authorized = process.env.DOC92_MYSQL_AUTHORIZED === 'SI';

function options() {
  const database = process.env.DOC92_MYSQL_DATABASE || '';
  assert.match(database, /^doc92_[a-z0-9_]+$/i, 'La base descartable debe iniciar por doc92_.');
  return {
    host: process.env.DOC92_MYSQL_HOST,
    port: Number(process.env.DOC92_MYSQL_PORT || 3306),
    user: process.env.DOC92_MYSQL_USER,
    password: process.env.DOC92_MYSQL_PASSWORD,
    database
  };
}

async function contender(config, challengeId) {
  const connection = await mysql.createConnection(config);
  try {
    await connection.beginTransaction();
    const [rows] = await connection.execute('SELECT Consumed,Attempts,ExpiresAtUtc FROM ra_auth_second_factor_challenge WHERE ChallengeId=? FOR UPDATE', [challengeId]);
    let affected = 0;
    if (rows.length === 1 && !rows[0].Consumed && rows[0].Attempts < 5) {
      const [result] = await connection.execute('UPDATE ra_auth_second_factor_challenge SET Consumed=1 WHERE ChallengeId=? AND Consumed=0 AND Attempts=0 AND ExpiresAtUtc>UTC_TIMESTAMP()', [challengeId]);
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

test('DOC-92: contrato existente y consumo concurrente en MySQL descartable', { skip: !authorized }, async () => {
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

    const [columns] = await connection.query("SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' ORDER BY ORDINAL_POSITION");
    assert.deepEqual(columns.map(row => row.COLUMN_NAME), ['Id','ChallengeId','AuthUserId','Provider','CodeHash','ExpiresAtUtc','Consumed','Attempts','CreatedAtUtc','AuthPayloadJson']);

    await connection.execute(`INSERT INTO ra_auth_second_factor_challenge
      (ChallengeId,AuthUserId,Provider,CodeHash,ExpiresAtUtc,Consumed,Attempts,CreatedAtUtc,AuthPayloadJson)
      VALUES (?,'7:11:RADICADOR:42','EMAIL','protected',UTC_TIMESTAMP()+INTERVAL 5 MINUTE,0,0,UTC_TIMESTAMP(),NULL)`, [challengeId]);
    const winners = await Promise.all([contender(config, challengeId), contender(config, challengeId)]);
    assert.equal(winners.reduce((sum, value) => sum + value, 0), 1, 'Exactamente un consumo debe ganar.');
  } finally {
    await connection.query('DROP TABLE IF EXISTS ra_auth_second_factor_challenge');
    await connection.end();
  }
});
