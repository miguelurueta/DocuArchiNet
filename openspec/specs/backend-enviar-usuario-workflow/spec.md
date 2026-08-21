# backend-enviar-usuario-workflow Specification

## Purpose

Permitir que una tarea Workflow se envíe a un usuario y actividad autorizados mediante contratos de servidor directos, privados, revalidados y auditables, sin depender de conectores ni de controles Web Forms.

## Requirements

### Requirement: Contrato directo y exclusivo de usuario

El sistema SHALL exponer `PreviewEnviarUsuario(idTarea, consulta, cursor, tamanoPagina)` y `EjecutarEnvioUsuario(solicitud)` mediante ASMX, usando contratos exclusivos de usuario que no acepten ni devuelvan `IdConector`.

#### Scenario: Payload sin conector

- **WHEN** el cliente solicita preview o ejecución
- **THEN** el contrato solo admite tarea, destino usuario–actividad y token donde corresponda, y la respuesta no serializa un conector.

### Requirement: Autorización calculada en servidor

El sistema SHALL resolver la identidad Gestión→Workflow y calcular `CAMBIO_USUARIO` en servidor, con denegación por defecto ante contexto o permiso inválido.

#### Scenario: Permiso ausente o contexto inválido

- **WHEN** el usuario no tiene `CAMBIO_USUARIO`, la sesión no es válida, la tarea no está activa o la ruta no está abierta
- **THEN** la operación devuelve un código funcional seguro y no revela datos de destino ni invoca el motor legacy.

### Requirement: Preview de destinos autorizado y de solo lectura

El sistema SHALL reducir los destinos al conjunto usuario–actividad autorizado por ruta, actividad, estado de usuario y `UTIL_ASIGNA_TAREA=1`, y después aplicar filtro parametrizado, cursor seguro, orden estable y tamaño máximo en servidor.

#### Scenario: Cursor, filtro o página inválidos

- **WHEN** el filtro supera el límite, el cursor no es válido o se solicita un tamaño fuera del máximo
- **THEN** el preview devuelve un bloqueo público sin SQL, excepciones, Session ni escrituras.

#### Scenario: Conjunto extenso

- **WHEN** existen más destinos autorizados que el tamaño de página
- **THEN** la respuesta devuelve únicamente la página solicitada, `TieneMas`, cursor siguiente y datos mínimos de selección.

### Requirement: Ejecución reautorizada y concurrente

El sistema SHALL tomar `GET_LOCK` por tarea y token y revalidar, dentro de ese lock, tarea, token, permiso, ruta/flujo, respuesta permitida, usuario destino, actividad destino, pertenencia a ruta, `UTIL_ASIGNA_TAREA` y notificación.

#### Scenario: Estado vencido o destino retirado

- **WHEN** el token no coincide, la respuesta requiere confirmación, el usuario deja de estar activo o el destino ya no pertenece a la ruta
- **THEN** la ejecución se bloquea antes de la llamada legacy.

#### Scenario: Solicitudes simultáneas

- **WHEN** dos solicitudes usan la misma tarea y token
- **THEN** solo una puede alcanzar el motor legacy y la otra recibe `WORKFLOW_TRANSITION_IN_PROGRESS` o un conflicto seguro.

### Requirement: Adaptador directo, resultados y auditoría

El sistema SHALL usar un único adaptador exclusivo para llamar una vez a `ClassWorkflow.Terminar_Tarea_Workflow` con `Page = Nothing`, sin conector ni reasignación de respuesta, y SHALL registrar auditoría sanitizada con mecanismo `ASMX_ENVIO_USUARIO`.

#### Scenario: Éxito con advertencia posterior

- **WHEN** el motor confirma el envío y correo o evento devuelve advertencia
- **THEN** el resultado permanece exitoso, contiene una advertencia pública y conserva una referencia de auditoría.

#### Scenario: Falla de auditoría

- **WHEN** la transición ya fue confirmada y la auditoría adicional falla
- **THEN** el resultado no revierte la transición y comunica una advertencia sanitizada.

### Requirement: Aislamiento y evidencia de regresión

El sistema SHALL conservar sin cambios `PreviewEnviarTarea`, `EjecutarEnvioTarea`, `ServicioTransicionTarea`, contratos por conector y el recorrido de Continuar flujo.

#### Scenario: Verificación focalizada

- **WHEN** se ejecutan las pruebas de esta etapa
- **THEN** cubren permisos, preview sin escrituras, paginación, validaciones, token, lock, advertencias y auditoría sin ejecutar E2E autenticado ni modificar configuración de ambiente.
