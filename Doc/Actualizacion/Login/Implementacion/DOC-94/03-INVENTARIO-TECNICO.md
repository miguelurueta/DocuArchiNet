# Inventario técnico

Repositorio: `DocuArchiNet`. Las rutas son relativas a su raíz.

## APIs y presentación

| Tipo | Ruta del archivo | Clase o interfaz | Nombre exacto | Parámetros y tipos | Retorno | Descripción | Relaciones o dependencias |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Endpoint WebForms | `gestor.aspx.vb` | `gestor` | `Button1_Click` | `sender As Object`, `e As EventArgs` | `Void` | Postback de login privado. No es API HTTP independiente. Autorización previa: No aplica. Entrada: controles WebForms. Salida: redirect o mensaje. | `ClassGestorSesion` |
| Método legacy | `Defaul/ClassGestorSesion.vb` | `ClassGestorSesion` | `InicioAplicacionWebGestorDocumental` | `modulo As String`, `user As String`, `passs As String`, `nombre_empresa As String` | `String` | Valida entrada/conexiones/credenciales, limpia contraseña y delega preautenticación. | `GestorModuleSesion`, servicio 2FA, Forms Authentication |
| Método legacy | misma ruta | `ClassGestorSesion` | `ValidaUserAplicacion` | `user As String`, `ByRef pasw As String`, `Nombre_Aplication As String`, `ByRef id_user As Integer` | `String` | Validación de credenciales existente, no reescrita. | Conexiones legacy por módulo |
| Método finalizador | misma ruta | `ClassGestorSesion` | `FinalizeLogin` | `context As LegacyLoginFinalizationContext` | `LegacyLoginFinalizationResult` | Ejecuta sesiones, relaciones, permisos y auditoría; no emite cookie ni redirect. | Implementa `ILegacyLoginFinalizer` |

No se agregó Controller, ASMX ni endpoint REST. Verbo HTTP, ruta completa, autorización y DTO de entrada/salida: No aplica.

## Servicios y contratos

| Tipo | Ruta del archivo | Clase o interfaz | Nombre exacto | Parámetros y tipos | Retorno | Descripción | Relaciones o dependencias |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Interfaz | `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb` | `ISecondFactorPreAuthenticationService` | `Execute` | `request As SecondFactorPreAuthenticationRequest` | `SecondFactorPreAuthenticationResult` | Puerto de decisión de preautenticación. | Implementada por el servicio. |
| Servicio | `Services/Login/SegundoFactor/SecondFactorPreAuthenticationService.vb` | `SecondFactorPreAuthenticationService` | `Execute` | `request As SecondFactorPreAuthenticationRequest` | `SecondFactorPreAuthenticationResult` | Resuelve módulo/principal; finaliza o se detiene. | Tres puertos inyectados. |
| Interfaz | `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb` | `ISecondFactorLoginModuleRepository` | `Resolve` | `companyName As String`, `moduleName As String` | `SecondFactorLoginModule` | Catálogo central autoritativo. | Implementada por ODBC. |
| Interfaz | misma ruta | `ISecondFactorPrincipalRepository` | `Resolve` | `context As ContextoPreautenticacionModulo` | `SecondFactorPrincipal` | Puerto común de principal. | Base MySQL. |
| Interfaz | misma ruta | `ISecondFactorPrincipalRepositoryResolver` | `Resolve` | `moduleType As String` | `ISecondFactorPrincipalRepository` | Selecciona adaptador por tipo central. | Resolver de infraestructura. |
| Interfaz | misma ruta | `ILegacyLoginFinalizer` | `FinalizeLogin` | `context As LegacyLoginFinalizationContext` | `LegacyLoginFinalizationResult` | Finalización sin dependencia de challenge. | `ClassGestorSesion` |

## Repositorios y conexiones

| Tipo | Ruta del archivo | Clase o interfaz | Nombre exacto | Parámetros y tipos | Retorno | Descripción | Relaciones o dependencias |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Repositorio ODBC | `Infrastructure/Repositories/Login/SegundoFactor/OdbcSecondFactorLoginModuleRepository.vb` | `OdbcSecondFactorLoginModuleRepository` | `Resolve` | `companyName As String`, `moduleName As String` | `SecondFactorLoginModule` | Consulta parametrizada con cardinalidad exacta sobre `empresa_gestion_documental` y `gestor_modulos`; lee la configuración 2FA del módulo. | `IModuleConnectionFactory`, `IDataExecutor` |
| Base repositorio | `Infrastructure/Repositories/Login/SegundoFactor/MySqlSecondFactorPrincipalRepositoryBase.vb` | `MySqlSecondFactorPrincipalRepositoryBase` | `Resolve` | `context As ContextoPreautenticacionModulo` | `SecondFactorPrincipal` | Consulta por `@login`, admite máximo una fila. | Factory y executor inyectados. |
| Adaptador | `Infrastructure/Repositories/Login/SegundoFactor/MySqlDocuarchiSecondFactorPrincipalRepository.vb` | `MySqlDocuarchiSecondFactorPrincipalRepository` | Constructor | `connections As IModuleConnectionFactory`, `executor As IDataExecutor` | No aplica | `usuarios_da(Clave_Usuario,idusuario,correo)`. | Base común. |
| Adaptador | `Infrastructure/Repositories/Login/SegundoFactor/MySqlGestorSecondFactorPrincipalRepository.vb` | `MySqlGestorSecondFactorPrincipalRepository` | Constructor | iguales | No aplica | `remit_dest_interno(Id_Remit_Dest_Int,Login_Usuario,Correo_Electronico)`. | Base común. |
| Adaptador | `Infrastructure/Repositories/Login/SegundoFactor/MySqlRadicacionSecondFactorPrincipalRepository.vb` | `MySqlRadicacionSecondFactorPrincipalRepository` | Constructor | iguales | No aplica | `usuario_radicador(id_usuario,Login_usuario,Correo_Usuario)`. | Base común. |
| Adaptador | `Infrastructure/Repositories/Login/SegundoFactor/MySqlWorkflowSecondFactorPrincipalRepository.vb` | `MySqlWorkflowSecondFactorPrincipalRepository` | Constructor | iguales | No aplica | `usuario_workflow(idU_suario,login_Usuario,Correo_Usuario)`. | Base común. |
| Resolver | `Infrastructure/Repositories/Login/SegundoFactor/SecondFactorPrincipalRepositoryResolver.vb` | `SecondFactorPrincipalRepositoryResolver` | `Resolve` | `moduleType As String` | `ISecondFactorPrincipalRepository` | Mapea cuatro tipos exactos. | Cuatro adaptadores inyectados. |
| Factory | `Infrastructure/Shared/Data/WorkflowModuleConnectionFactory.vb` | `GestorCatalogOdbcConnectionFactory` | `CreateOpenConnection` | `contexto As ContextoModulo` | `IDbConnection` | Abre snapshot ODBC central. | `OdbcConnection` |
| Factories | misma ruta | `WorkflowModuleConnectionFactory`, `DocuarchiModuleConnectionFactory`, `RadicacionModuleConnectionFactory`, `GestorModuleConnectionFactory` | `CreateOpenConnection` heredado | `contexto As ContextoModulo` | `IDbConnection` | Abren snapshots MySQL ya resueltos por Presentation. | `ModuleSnapshotConnectionFactory` |

Ningún repositorio nuevo accede a `HttpContext`, Session ni clases `conect`.

## Modelos y propiedades

| Tipo | Ruta del archivo | Clase o interfaz | Nombre exacto | Parámetros y tipos | Retorno | Descripción | Relaciones o dependencias |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Enum | `Modelo/Login/SegundoFactor/SegundoFactorModels.vb` | `SecondFactorPreAuthenticationStatus` | `FINALIZED`, `SECOND_FACTOR_REQUIRED`, `REJECTED` | No aplica | No aplica | Estados internos. | Resultado de servicio. |
| Modelo | misma ruta | `LegacyLoginFinalizationContext` | Constructor | `empresaId As Integer`, `moduloId As Integer`, `moduleType As String`, `internalUserId As Long`, `normalizedLogin As String` | No aplica | Contexto inmutable sin challenge. | Finalizador. |
| Modelo | misma ruta | `SecondFactorLoginModule` | Constructor | `empresaId As Integer`, `moduloId As Integer`, `moduleName As String`, `moduleType As String`, `configuration As SegundoFactorConfiguration` | No aplica | Snapshot central. | Repositorio central. |
| Modelo | misma ruta | `SecondFactorPrincipal` | Constructor | `internalUserId As Long`, `normalizedLogin As String`, `emailAddress As String` | No aplica | Identidad canónica; correo crudo es `Friend` y solo es obligatorio con 2FA activo; el destino se enmascara cuando existe. | Adaptadores y servicio. |
| Modelo | misma ruta | `SecondFactorPreAuthenticationRequest` | Constructor | `companyName As String`, `moduleName As String`, `validatedLogin As String`, `principalContext As ContextoPreautenticacionModulo` | No aplica | Entrada interna posterior a credenciales. | Servicio. |
| Modelo | misma ruta | `SecondFactorPreAuthenticationResult` | Constructor | `status`, `publicCode`, `loginModule`, `principal`, `finalization` | No aplica | Resultado interno sin contraseña ni challenge. | Wrapper legacy. |
| Modelo | misma ruta | `LegacyLoginFinalizationResult` | Propiedades | No aplica | No aplica | `Success`, `RedirectRequired`, `LocalRoute`, `PublicCode`. | Servicio y wrapper. |
| Contexto | `Domain/Shared/ContextoPreautenticacionModulo.vb` | `ContextoPreautenticacionModulo` | `EsValido` | ninguno | `Boolean` | Admite ID 0 solo para resolver principal. | Hereda `ContextoModulo`. |

## Obligatoriedad y validaciones

| Modelo | Propiedad | Tipo | Obligatoria | Validación |
| --- | --- | --- | --- | --- |
| `LegacyLoginFinalizationContext` | `EmpresaId`, `ModuloId` | `Integer` | Sí | Mayores que cero. |
| mismo | `ModuleType`, `NormalizedLogin` | `String` | Sí | No vacío; normalizados a mayúsculas. |
| mismo | `InternalUserId` | `Long` | Sí | Mayor que cero. |
| `SecondFactorLoginModule` | `Configuration` | `SegundoFactorConfiguration` | Sí | No nula; constructor valida bandera/proveedor/expiración. |
| `SecondFactorPrincipal` | `EmailAddress` | `String` (`Friend`) | Solo con 2FA activo | Si existe, su formato se valida al generar la máscara; vacío no bloquea `0`/`NULL`; no forma parte de DTO público. |
| `SecondFactorPreAuthenticationRequest` | nombres/login/contexto | varios | Sí | Texto no vacío y `ContextoPreautenticacionModulo.EsValido()`. |
| `SecondFactorPreAuthenticationResult` | `Status`, `PublicCode` | enum/String | Sí | Enum definido y código no vacío. |

DTOs HTTP: No aplica, porque DOC-94 no incorpora endpoint. `SegundoFactorEstadoDto` y `SegundoFactorResultadoDto` de DOC-91 no se modifican ni se usan todavía en este recorrido.
