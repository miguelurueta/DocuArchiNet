const test = require('node:test');
const assert = require('node:assert/strict');
const guardModule = require('../js/workflow/importar-servicio-web/importar-servicio-web-task-context-guard.js');
const documentList = require('../js/workflow/importar-servicio-web/importar-servicio-web-document-list-adapter.js');
test('señal multi-pestaña exige verificación y no proyecta otra tarea', () => { let currentTask=1001, inserted=0; const guard=guardModule.create({currentTaskId:()=>currentTask}); guard.capture({TaskId:1001,IntentId:'intent-1'}); assert.equal(guard.observeTaskSignal({TaskId:2002}),true); currentTask=2002; const adapter=documentList.create({currentTaskId:()=>currentTask,appendDocument:()=>{inserted++;return true}}); assert.deepEqual(adapter.synchronize({items:[{status:'Disponible',taskId:1001,documentId:90}]}).documents,[]); assert.equal(inserted,0); });
