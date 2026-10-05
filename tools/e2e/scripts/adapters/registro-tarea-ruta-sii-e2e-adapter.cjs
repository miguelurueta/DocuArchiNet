'use strict';

const REGISTRO_TAREA_RUTA_SII_E2E_ADAPTER = Object.freeze({
  id: 'registro-tarea-ruta-sii',
  servicePath: 'webservice/WebServiceWorkflow.asmx',
  operations: Object.freeze({
    register: Object.freeze({ id: 'Service_registro_tarea_ruta_sii', payload: Object.freeze(['parameter']) })
  }),
  async executeExecution() {
    return Object.freeze({
      codes: Object.freeze({ platform: 'READY_FOR_UI' }),
      count: 0,
      latenciesMs: Object.freeze([])
    });
  }
});

module.exports = { REGISTRO_TAREA_RUTA_SII_E2E_ADAPTER };
