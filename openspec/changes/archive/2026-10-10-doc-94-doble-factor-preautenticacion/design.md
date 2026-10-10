<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10,D-11 -->
## Context

El postback `gestor.aspx.vb::Button1_Click` delega en `ClassGestorSesion.InicioAplicacionWebGestorDocumental`. Ese método prepara conexiones, resuelve el tipo de módulo, valida credenciales y, desde el primer bloque posterior al resultado `YES`, crea todo el estado autenticado. El punto quirúrgico de separación está después de `ValidaUserAplicacion` y antes de la primera inicialización específica de módulo.

Flujo actual verificado:

```text
gestor.aspx::Button1_Click
  -> ClassGestorSesion.InicioAplicacionWebGestorDocumental(modulo, user, pass, empresa)
     -> GestorModuleSesion.InicializaconexionesModulos(modulo, empresa)
     -> GestorModuleSesion.Retorna_tipo_modulo(modulo)
     -> ClassGestorSesion.ValidaUserAplicacion(user, ByRef pass, tipo, ByRef id)
     -> [bloque por tipo: relaciones + permisos + sesiones + auditoría]
     -> FormsAuthentication.RedirectFromLoginPage(user, False)
```

`ContextoModulo.EsValido()` exige un ID de usuario positivo, pero la conexión del módulo debe abrirse antes de resolver ese ID. Además, los IDs guardados en Session no representan uniformemente el módulo seleccionado. El repositorio central debe resolver la selección de manera autoritativa y el contexto de preautenticación no puede fingir un ID.

## Goals / Non-Goals

**Goals**

- Separar credenciales, decisión 2FA y finalización sin cambiar reglas legacy.
- Preservar exactamente el recorrido con 2FA apagado.
- Detener toda autenticación definitiva con 2FA activo.
- Resolver configuración e identidad con datos del servidor y SQL parametrizado.

**Non-Goals**

- Crear/verificar challenge, enviar correo, exponer ASMX o cambiar markup/JavaScript.
- Modernizar contraseñas, SQL legacy no afectado o la arquitectura completa de sesión.
- Cambiar recuperación de contraseña o `ClassCorreo`.
- Agregar tablas, columnas o migraciones.

## Decisions

### D-01 — Frontera de DOC-94

El resultado activo es interno: código `SECOND_FACTOR_REQUIRED`, identidad canónica, correo enmascarado y configuración efectiva. No se persiste challenge ni se notifica. Así se evita adelantar contratos de las tareas de challenge/SMTP/UI.

### D-02 — Validación legacy sin reescritura

Se mantienen `InicializaconexionesModulos(String,String)`, `Retorna_tipo_modulo(String)` y `ValidaUserAplicacion(String, ByRef String, String, ByRef Integer)`. Gestor/Workflow conservan su validación de estado; Radicación/DocuArchi no reciben una nueva regla.

### D-03 — Finalizador único sin redirect

El bloque actual posterior a credenciales se mueve, sin duplicarlo, a un método que devuelve `LegacyLoginFinalizationResult`. El método no emite cookie ni redirige. El wrapper `InicioAplicacionWebGestorDocumental` conserva `FormsAuthentication.RedirectFromLoginPage` solo para finalización exitosa.

### D-04 — Contexto de finalización independiente

Se agrega `LegacyLoginFinalizationContext(empresaId As Integer, moduloId As Integer, moduleType As String, internalUserId As Long, normalizedLogin As String)` y `ILegacyLoginFinalizer.FinalizeLogin(context As LegacyLoginFinalizationContext) As LegacyLoginFinalizationResult`. `PendingSecondFactorContext` continúa representando exclusivamente un challenge futuro.

### D-05 — Configuración central autoritativa

Un repositorio ODBC usa la conexión central existente `OdbcServicesGestor`, parámetros y cardinalidad exacta para resolver empresa, módulo, tipo y las columnas `RequiereSegundoFactor`, `SecondFactorProviderType` y `SegundoFactorTiempoExpira`. No acepta IDs del navegador ni confía en los IDs heterogéneos de Session.

### D-06 — Cuatro adaptadores de identidad

Cada adaptador tiene SQL fijo y parametrizado, y devuelve exactamente una identidad canónica:

| Módulo | Tabla | ID | Login | Correo |
| --- | --- | --- | --- | --- |
| DocuArchi | `usuarios_da` | `Clave_Usuario` | `idusuario` | `correo` |
| Gestor | `remit_dest_interno` | `Id_Remit_Dest_Int` | `Login_Usuario` | `Correo_Electronico` |
| Radicación | `usuario_radicador` | `id_usuario` | `Login_usuario` | `Correo_Usuario` |
| Workflow | `usuario_workflow` | `idU_suario` | `login_Usuario` | `Correo_Usuario` |

Cero o múltiples filas e ID inválido son errores controlados en cualquier rama. El correo solo es obligatorio y se valida cuando `RequiereSegundoFactor = 1`; con `0`/`NULL` su ausencia no bloquea el finalizador legacy.

### D-07 — Contexto de conexión preautenticada

`ContextoPreautenticacionModulo` hereda de `ContextoModulo` y sobrescribe `EsValido()` únicamente para admitir `IdUsuario = 0` en esta fase. No se modifica la regla base. Presentation resuelve los snapshots de conexión desde Session y los inyecta en factories/repositorios; ningún repositorio conoce `HttpContext`.

### D-08 — Matriz de decisión

| Configuración | Acción | Efectos permitidos |
| --- | --- | --- |
| `0` o `NULL` | Resolver ID/login, sin exigir correo; ejecutar finalizador; wrapper redirige | Los mismos del login legacy |
| `1` | Resolver identidad y correo válido; devolver `SECOND_FACTOR_REQUIRED` | Solo conexiones preparadas y resultado mínimo |
| Otro valor/error/cardinalidad inválida | Fallar cerrado con mensaje sanitizado | Ningún efecto autenticado |

### D-09 — Secretos y límites

La variable local de contraseña se limpia inmediatamente después de `ValidaUserAplicacion`. Los nuevos repositorios reciben contextos/snapshots tipados. No se toca `Button_Aceptar_Click` de recuperación ni `ClassCorreo`.

### D-10 — Estrategia de pruebas

Dobles deterministas verifican cuatro módulos, `0`/`NULL` incluso sin correo, activo con correo válido o ausente, credenciales inválidas, configuración inválida, identidad inexistente y el mismo login en empresas diferentes. La rama activa verifica cero llamadas al finalizador, cero cookie, cero auditoría y cero sesiones autenticadas. No se usa DB/E2E real sin autorización vigente.

### D-11 — Documentación verificable

Se crea `Doc/Actualizacion/Login/Implementacion/DOC-94/` con flujo, matriz, inventario de contratos y trazabilidad a código. El manifiesto de diagramas y la validación estructural se actualizan solo después de existir los símbolos finales.

## Rutas y contratos previstos

| Capa | Ruta | Símbolo/uso |
| --- | --- | --- |
| Presentación | `gestor.aspx.vb` | `Button1_Click`; consume resultado, sin markup OTP |
| Legacy | `Defaul/ClassGestorSesion.vb` | wrapper, validación intacta y finalizador extraído |
| Legacy | `Defaul/GestorModuleSesion.vb` | lecturas actuales; modificar solo si es imprescindible exponer snapshot |
| Modelo | `Modelo/Login/SegundoFactor/SegundoFactorModels.vb` | `LegacyLoginFinalizationContext`, resultado de preautenticación |
| Contratos | `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb` | servicio, repositorio central, resolvedor/adaptadores y finalizador |
| Dominio | `Domain/Shared/ContextoPreautenticacionModulo.vb` | contexto válido antes de conocer ID |
| Aplicación | `Services/Login/SegundoFactor/SecondFactorPreAuthenticationService.vb` | decisión 2FA |
| Infraestructura | `Infrastructure/Repositories/Login/SegundoFactor/OdbcSecondFactorLoginModuleRepository.vb` | catálogo central |
| Infraestructura | `Infrastructure/Repositories/Login/SegundoFactor/MySql*SecondFactorPrincipalRepository.vb` | cuatro adaptadores |
| Conexión | `Infrastructure/Shared/Data/WorkflowModuleConnectionFactory.vb` | factories por snapshot; agregar Gestor si falta |
| Proyecto | `GestionDocumental-Docuarchi.net.vbproj` | incluir archivos VB/pruebas necesarias |
| Pruebas | `tests/` y `tools/validation/` | caracterización, unidad y regresión documental |
| Documentación | `Doc/Actualizacion/Login/Implementacion/DOC-94/` | arquitectura implementada y evidencia |

`webservice/WebServiceLoginSegundoFactor.asmx(.vb)` queda reservado y no se crea/modifica.

## Risks / Trade-offs

- **Activación prematura:** un valor `1` detiene el login porque DOC-94 aún no ofrece OTP. Mitigación: desplegar solo tras comprobar por `SELECT` autorizado que los módulos objetivo están en `0`/`NULL`.
- **Gran bloque legacy:** la extracción puede omitir un efecto o alterar su orden. Mitigación: caracterización por módulo antes y después, y movimiento mecánico del bloque.
- **IDs de Session ambiguos:** podrían apuntar a otro módulo. Mitigación: resolución central por empresa/módulo seleccionado y contexto tipado.
- **Contrato DOC-91:** cambiar `ILegacyLoginFinalizer` rompe dobles/consumidores. Mitigación: actualizar todos los usos en la misma unidad y ejecutar suites DOC-91/92/93.

## Migration Plan

1. Caracterizar flujo y efectos actuales por módulo.
2. Ajustar modelos/interfaces y contextos sin conectar todavía la UI.
3. Implementar repositorio central y cuatro adaptadores con dobles.
4. Extraer el finalizador mecánicamente y probar equivalencia.
5. Conectar el servicio al wrapper; mantener redirección solo en rama apagada.
6. Ejecutar validaciones, build y pruebas; documentar resultados.
7. Antes de desplegar, confirmar configuración real mediante `SELECT` expresamente autorizado.

## Open Questions

- Ninguna de diseño. La configuración efectiva de cada ambiente es evidencia operativa pendiente y no se presume.
