'use strict';

const PRODUCTION_DOCUMENT_UPLOAD_E2E_ADAPTER = Object.freeze({
  id: 'production-document-upload',
  servicePath: 'webservice/WebServiceProducion.asmx',
  operations: Object.freeze({
    prepare: Object.freeze({ id: 'ServiceSolicitaCargarDocumentoExpediente', payload: Object.freeze(['parameter']) })
  }),
  async executePreview() {
    return Object.freeze({
      codes: Object.freeze({ platform: 'READY_FOR_NON_MUTATING_UI' }),
      count: 0,
      latenciesMs: Object.freeze([])
    });
  },
  async executeExecution() {
    return Object.freeze({
      codes: Object.freeze({ platform: 'READY_FOR_REAL_STORAGE_UI' }),
      count: 0,
      latenciesMs: Object.freeze([])
    });
  }
});

module.exports = { PRODUCTION_DOCUMENT_UPLOAD_E2E_ADAPTER };
