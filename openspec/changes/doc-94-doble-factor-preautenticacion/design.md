## Context

DOC-94: DOBLE-FACTOR-PREAUTENTICACION

## Jira Details

> # 04 — Preautenticación, finalizador legacy y cuatro módulos
> 
> ## ROL ESPERADO
> 
> Actúa como arquitecto full stack senior especialista en WebForms legacy.
> 
> ## OBJETIVO
> 
> Separar quirúrgicamente, dentro del OpenSpec propio de esta Jira, la validación de credenciales de la finalización del login y agregar adaptadores de identidad/correo para los cuatro módulos, sin activar todavía el challenge ni cambiar la UI.
> 
> ## CONTEXTO
> 
> Lee la exploración y revisa completos `gestor.aspx.vb`, `Defaul/ClassGestorSesion.vb`, `Defaul/GestorModuleSesion.vb`, `Defaul/conect.vb`, `Global.asax.vb`, `Web.config`, factories de conexión, permisos, auditoría y redirecciones. Caracteriza primero el recorrido actual de cada módulo.
> 
> ## PRECONDICIONES DE RUTAS
> 
> El `design.md` debe inventariar los archivos exactos, métodos legacy afectados, clases y firmas. Usa:
> 
> - `Services/Login/SegundoFactor/` para preautenticación, evaluación de configuración y el contrato de finalización como casos de uso.
> - `Infrastructure/Repositories/Login/SegundoFactor/` para configuración central y adaptadores MySQL de identidad/correo por módulo.
> - `Modelo/Login/SegundoFactor/` únicamente si una interfaz o resultado de Application no existe todavía.
> - `Defaul/ClassGestorSesion.vb` para extraer/delegar el finalizador sin trasladar toda la clase; `Defaul/GestorModuleSesion.vb` solo si una lectura existente debe exponerse sin duplicación.
> - `gestor.aspx.vb` únicamente para conectar temporalmente el resultado de preautenticación; no modificar todavía markup/JavaScript OTP.
> - `tests/`, `tools/validation/` y `GestionDocumental-Docuarchi.net.vbproj` según corresponda.
> - `webservice/WebServiceLoginSegundoFactor.asmx(.vb)` es la frontera reservada para la tarea 05; esta tarea no la crea ni modifica.
> 
> No crear SQL en `Defaul`, lógica de sesión en repositorios, modelos dentro de `Services` ni un nuevo code-behind de login.
> 
> ## REQUISITOS POSITIVOS Y CONTRATOS
> 
> 1. Mantener la preparación existente de conexiones de módulo en Session antes de credenciales; documentar que no representa autenticación.
> 2. Reutilizar literalmente las reglas de `ValidaUserAplicacion`: Gestor/Workflow conservan su validación de estado; Radicación/DocuArchi no reciben una nueva regla silenciosa.
> 3. Extraer el bloque posterior a credenciales a un único finalizador tipado que inicialice exactamente usuarios relacionados, permisos, auditoría y sesiones por tipo de módulo.
> 4. Mantener un wrapper legacy que invoque ese finalizador y luego `FormsAuthentication.RedirectFromLoginPage`, preservando el recorrido con 2FA apagado.
> 5. Preparar un modo de finalización sin redirect para la futura llamada ASMX; todavía no debe establecer cookie ni quedar públicamente invocable.
> 6. Implementar adaptadores parametrizados para resolver ID interno y correo desde:
> 
> | Módulo | Tabla | ID | Login | Correo |
> |---|---|---|---|---|
> | DocuArchi | `usuarios_da` | `Clave_Usuario` | `idusuario` | `correo` |
> | Gestor | `remit_dest_interno` | `Id_Remit_Dest_Int` | `Login_Usuario` | `Correo_Electronico` |
> | Radicación | `usuario_radicador` | `id_usuario` | `Login_usuario` | `Correo_Usuario` |
> | Workflow | `usuario_workflow` | `idU_suario` | `login_Usuario` | `Correo_Usuario` |
> 
> 7. Leer del catálogo central `gestor_modulos` un snapshot tipado de `RequiereSegundoFactor`, proveedor y expiración. Con `0`/`NULL`, finalizar inmediatamente como hoy; con `1`, construir solo el resultado interno de preautenticación y no finalizar. Valores inválidos siguen la política cerrada de la exploración.
> 8. Los repositorios reciben snapshots de conexión; no consultan directamente Session/HttpContext.
> 
> ## RESTRICCIONES CRITICAS Y REGLAS DE ANTIRREGRESION
> 
> - Antes del OTP pueden existir conexiones preparadas en Session, pero no usuario autenticado, permisos, relaciones, auditoría de ingreso ni cookie.
> - No conservar contraseña después de validarla.
> - No aceptar desde navegador ID, correo, tipo, empresa o módulo como autoridad.
> - No reescribir algoritmos legacy de contraseña ni concatenaciones ajenas a las nuevas consultas; una modernización general queda fuera de alcance.
> - Recuperación de contraseña y `ClassCorreo` permanecen intactas.
> 
> ## FLUJO FUNCIONAL
> 
> ```text
> gestor.aspx postback
>   -> preparar snapshots de conexión legacy
>   -> ValidaUserAplicacion sin cambiar reglas
>   -> leer configuración central 2FA
>   -> desactivado: finalizador único -> RedirectFromLoginPage
>   -> activado: resolver ID/correo -> contexto mínimo -> detener sin autenticar
> ```
> 
> ## PRUEBAS OBLIGATORIAS
> 
> Construye una línea de caracterización por los cuatro módulos que compare el recorrido anterior con el refactor cuando 2FA está `0`/`NULL`: mismas Session relevantes, permisos, auditoría y destino. Con 2FA simulado activo, demuestra que credenciales válidas producen preautenticación mínima y cero efectos autenticados. Cubre credenciales inválidas, ID/correo inexistente, configuración inválida y logins iguales en empresas distintas.
> 
> Ejecuta las pruebas de tareas previas y MSBuild. No accedas a cuentas/bases reales sin autorización expresa actual.
> 
> ## CRITERIOS DE ACEPTACION
> 
> - Existe un solo finalizador para evitar divergencia entre login legacy y futuro OTP.
> - Los cuatro adaptadores resuelven identidad inequívoca con SQL parametrizado.
> - El recorrido desactivado es funcionalmente equivalente al original.
> - El recorrido activo se detiene antes de toda autenticación definitiva.
> - El OpenSpec traza métodos afectados, `D-XX`, `RQ-XX`, pruebas y riesgos.
> 
> ## DOCUMENTACION TECNICA
> 
> Actualiza la documentación existente del login y su matriz por módulo. Documenta el contrato de cada interfaz/método: nombre exacto, parámetros y tipos, retorno, estados/error y efectos permitidos. Si falta un documento, crea el índice bajo `Doc/Actualizacion/Login/Implementacion/<JIRA>/` y enlázalo desde el OpenSpec.
> 
> ## ENTREGABLE FINAL
> 
> Entrega firmas exactas de preautenticación/finalizador/adaptadores, archivos, pruebas unitarias con ruta y caso, build, comandos, resultados y riesgos.

## Goals / Non-Goals

**Goals**
- Refinar alcance tecnico usando el contexto completo de Jira.
- Definir decisiones arquitectonicas, riesgos y plan de migracion.

**Non-Goals**
- Cambios fuera del alcance descrito por el ticket.

## Decisions

1. Las decisiones funcionales y tecnicas se completan durante `opsxj:refine`; no se inyectan politicas de otro perfil tecnologico.


## Risks / Trade-offs

- El refinamiento debe identificar compatibilidad, riesgos y limites del modulo afectado antes de iniciar cambios.

## Migration Plan

1. Completar y aprobar `refinement.md` antes de marcar tareas de implementacion.
2. Sincronizar cada decision con design, spec y tasks mediante `opsxj:refine --sync`.

## Open Questions

- TBD
