## ADDED Requirements

### Requirement: Normalizacion canonica de claims de autorizacion
El frontend SHALL construir una representacion canonica de autorizacion a partir del contrato de autenticacion, de modo que los consumidores de UI no dependan del formato raw del token o de la respuesta backend.

#### Scenario: Claims nuevos presentes en autenticacion
- **WHEN** una autenticacion exitosa incluya claims JWT del contrato nuevo
- **THEN** el sistema MUST normalizar esos claims a la representacion canonica usada por hooks y guards

#### Scenario: Rehidratacion de sesion con token valido
- **WHEN** la aplicacion rehidrate una sesion valida desde almacenamiento local
- **THEN** el sistema MUST recomputar la representacion canonica de claims de forma deterministica

### Requirement: Compatibilidad temporal con permisos legacy
Durante la migracion, el frontend MUST soportar el payload legacy basado en `usuario.permisos` cuando la fuente nueva de claims no este disponible.

#### Scenario: Respuesta legacy sin claims nuevos
- **WHEN** una respuesta de autenticacion no incluya claims nuevos pero incluya `usuario.permisos`
- **THEN** el sistema MUST derivar permisos efectivos desde `usuario.permisos` para mantener autorizacion funcional

#### Scenario: Fallback sin ruptura de sesion
- **WHEN** se active el camino de compatibilidad legacy
- **THEN** el sistema MUST preservar login, refresco de claims y evaluacion de acceso sin errores de contrato

### Requirement: Validacion de contrato Auth en modo compatible
La validacion de autenticacion SHALL aceptar tanto el contrato nuevo como el contrato legacy permitido por migracion, y MUST fallar solo cuando faltan campos criticos para establecer sesion/autorizacion.

#### Scenario: Contrato nuevo valido
- **WHEN** el backend devuelva token, expiracion y estructura de claims del contrato nuevo
- **THEN** la validacion MUST marcar la respuesta como valida sin exigir `usuario.permisos`

#### Scenario: Contrato invalido sin datos minimos
- **WHEN** una respuesta de autenticacion no incluya campos minimos para sesion o autorizacion
- **THEN** la validacion MUST rechazar la respuesta con errores de contrato accionables

### Requirement: Consumo estable de autorizacion en UI protegida
Los mecanismos de autorizacion del frontend (hooks, rutas protegidas y guards de acciones) MUST evaluar permisos sobre la representacion canonica y no sobre detalles de origen del claim.

#### Scenario: Ruta protegida con permiso requerido
- **WHEN** una ruta protegida solicite uno o mas permisos
- **THEN** el sistema MUST autorizar o denegar usando la representacion canonica normalizada

#### Scenario: Guard de accion con claimKey y reglas requeridas
- **WHEN** un guard de acciones dinamicas evalua `claimKey`, `requiredAny` o `requiredAll`
- **THEN** el sistema MUST aplicar las reglas contra permisos efectivos normalizados de forma consistente
