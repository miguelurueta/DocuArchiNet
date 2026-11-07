'use strict';
const {loginObservation}=require('../../tests/support/doc111-importacion-carpetas.cjs');
const codes=new Set(`LOGIN_TRANSPORT_FAILED LOGIN_CREDENTIALS_REJECTED LOGIN_ACCOUNT_NOT_FOUND
LOGIN_ACCOUNT_BLOCKED LOGIN_INPUT_REQUIRED LOGIN_SECOND_FACTOR_REQUIRED LOGIN_SERVICE_REJECTED
LOGIN_SERVER_REJECTED LOGIN_POSTBACK_UNAVAILABLE LOGIN_HTTP_REJECTED PRODUCTION_OPEN_FAILED
AUTHENTICATED_PAGE_UNAVAILABLE IMPORT_CONTROL_UNAVAILABLE TOKEN_UNAVAILABLE SESSION_NOT_ESTABLISHED
IMPORT_ACCESS_DENIED IMPORT_CAPABILITY_UNAVAILABLE AUTHORIZATION_REQUIRED RESPONSE_INVALID
OPERATION_NOT_CONFIRMED READONLY_OPERATION_REQUIRED`.trim().split(/\s+/).map(c=>'DOC111_'+c));
codes.add('DOC116_LEVEL_SELECTION_FAILED');codes.add('DOC116_LEVEL_REFERENCE_INVALID');
const serverCodes=new Set('CAPACIDAD_NO_DISPONIBLE ACCESO_DENEGADO SOLICITUD_INVALIDA SESION_REQUERIDA CSRF_INVALIDO ORIGEN_INVALIDO ERROR_INTERNO CONFIGURACION_INVALIDA OPERACION_EN_CURSO CONFLICTO_REVISION ARCHIVO_RECHAZADO RESULTADO_INCIERTO NO_CLASIFICADO'.split(' '));
function preflightFailure(error){
 const result={code:'UNCLASSIFIED_FAILURE',loginObservation:null,operation:null,httpStatus:null,serverCode:null};
 for(const key of ['code','message'])try{const code=error?.[key];if(codes.has(code)){result.code=code;break;}}catch{}
 try{result.loginObservation=loginObservation(error?.loginObservation);}catch{}
 try{if(error?.operation==='ConsultarLote'){result.operation='ConsultarLote';if(Number.isInteger(error.httpStatus)&&error.httpStatus>=100&&error.httpStatus<=599)result.httpStatus=error.httpStatus;if(serverCodes.has(error.serverCode))result.serverCode=error.serverCode;}}catch{}
 return result;
}
module.exports={preflightFailure};
