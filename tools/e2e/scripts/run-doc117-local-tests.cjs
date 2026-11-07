'use strict';
const fs=require('node:fs'),path=require('node:path'),{spawn}=require('node:child_process');
const root=path.resolve(__dirname,'../../..');
const files=require('../fixtures/doc117-local-tests.json');
if(!Array.isArray(files)||new Set(files).size!==files.length||files.some(file=>typeof file!=='string'||!/^(?:tests|tools\/e2e\/tests)\/[A-Za-z0-9_-]+\.test\.cjs$/.test(file))){throw Error('DOC117_LOCAL_TEST_INVENTORY_INVALID');}
fs.mkdirSync(path.join(root,'.tmp'),{recursive:true});
const child=spawn(process.execPath,['--test','--test-reporter=tap',...files],{cwd:root,stdio:'inherit',windowsHide:true});
child.once('error',()=>{console.error('DOC117_LOCAL_TESTS_NOT_STARTED');process.exitCode=2;});
child.once('exit',code=>{process.exitCode=code??2;});
