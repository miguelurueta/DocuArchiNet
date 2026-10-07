'use strict';

const SCANNER_LINK_OVERLAY_E2E_ADAPTER = Object.freeze({
  id: 'scanner-link-overlay',
  servicePath: 'webservice/WebServiceWorkflowModern.asmx',
  operations: Object.freeze({
    observe: Object.freeze({ id: 'PreviewEnviarTarea', payload: Object.freeze(['idTarea', 'filtro', 'cursor', 'tamanoPagina']) })
  }),
  async executeExecution() {
    return Object.freeze({
      codes: Object.freeze({ platform: 'READY_FOR_SCANNER_LINK_UI' }),
      count: 0,
      latenciesMs: Object.freeze([])
    });
  }
});

module.exports = { SCANNER_LINK_OVERLAY_E2E_ADAPTER };
