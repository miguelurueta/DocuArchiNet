# Prompt base obligatorio — 2FA compatible para DocuArchiNet

Adjuntar este contenido al inicio de cada etapa.

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior especializado en VB.NET, ASP.NET Web Forms, .NET Framework 4.6.1, Forms Authentication, MySQL y seguridad de autenticación. Trabaja quirúrgicamente sobre el código existente, con cambios pequeños, trazables y reversibles.

## OBJETIVO GLOBAL

Incorporar segundo factor por correo al login existente de DocuArchiNet para:

- `DOCUARCHI CONTENEDOR`;
- `GESTOR DOCUMENTAL`;
- `RADICACION DOCUMENTAL`;
- `WORKFLOW DOCUMENTAL`.

Cuando `RequiereSegundoFactor` esté desactivado, el recorrido, permisos, Session, auditoría, redirección y mensajes actuales deben conservarse. Cuando esté activado, las credenciales válidas solo producen una preautenticación; Forms Authentication y la sesión autenticada final se crean después de verificar el OTP.

## FUENTES OBLIGATORIAS

Antes de editar:

1. Leer `../Exploracion/exploracion-doble-factor-autenticacion.md` completo.
2. Inspeccionar las implementaciones vigentes; no asumir que las rutas o firmas siguen iguales.
3. Revisar `gestor.aspx.vb`, `Defaul/ClassGestorSesion.vb`, `Defaul/GestorModuleSesion.vb`, `radicador/ClassCorreo.vb`, configuración, proyecto y pruebas relacionadas.
4. Para E2E autenticado, leer primero `tools/e2e/AGENT-RUNBOOK.md` y obtener las autorizaciones exigidas.

## DECISIONES ARQUITECTONICAS FIJAS

- `docuarchi.gestor_modulos` es la fuente central de configuración 2FA.
- `docuarchi.ra_auth_second_factor_challenge` es la persistencia central del challenge.
- Identidad y correo se resuelven en la base correspondiente al módulo y empresa.
- El correo debe enviarse mediante `ClassCorreo.Envio_Correo_recuperacion_pasword`; no crear otro `SmtpClient` ni incorporar `EmailSenderStub`.
- `gestor.aspx` continúa siendo la única entrada de credenciales. El ASMX de segundo factor opera únicamente una preautenticación pendiente de la misma Session y nunca acepta usuario/contraseña.
- Mantener Forms Authentication y la inicialización legacy necesaria; no introducir JWT.
- No copiar ensamblados .NET 9/10 del Core al proyecto .NET Framework 4.6.1.
- Solo EMAIL entra en este alcance. TOTP y recuperación de contraseña quedan fuera.
- No guardar el contexto completo de autenticación, contraseñas, conexiones, cookies, OTP en claro ni secretos en `AuthPayloadJson`, logs o errores.

## RESTRICCIONES CRITICAS

- No reescribir el login completo ni refactorizar módulos no relacionados.
- No cambiar consultas, permisos o reglas de negocio de los cuatro módulos salvo el corte explícito entre preautenticación y finalización.
- No abrir sesión autenticada, emitir cookie Forms Authentication, registrar inicio exitoso ni redirigir al destino final antes del OTP correcto.
- No permitir bypass por refresh, back, doble POST, cambio de módulo/empresa, challenge de otro usuario o sesión, correo ausente o fallo del proveedor.
- No permitir que el cliente seleccione arbitrariamente un challenge: el servidor resuelve el challenge pendiente desde un contexto mínimo ligado a la Session.
- No usar el navegador como autoridad sobre usuario, módulo, empresa, correo, expiración o intentos.
- No debilitar hashing de contraseñas ni modificar su almacenamiento.
- El OTP debe protegerse con HMAC y secreto externo a la base; comparación en tiempo constante. Nunca incluir el secreto en código o documentación.
- Creación, intentos, invalidación y consumo deben ser seguros ante concurrencia.
- No aplicar DDL ni cambiar valores 2FA de ningún ambiente sin autorización expresa. Preparar scripts versionados es distinto de ejecutarlos.
- No ejecutar E2E real, envío real de correo o pruebas de carga sin autorización explícita del ambiente y las cuentas.
- No imprimir ni persistir credenciales, cookies, cadenas de conexión, OTP o información personal innecesaria.

## REGLAS DE COMPATIBILIDAD Y ANTIRREGRESION

- Capturar una línea base del login sin 2FA antes de cambiarlo.
- Preservar firmas públicas cuando sea posible; si una firma debe cambiar, mantener adaptador compatible y documentarlo.
- El valor seguro por defecto para configuración ausente o incoherente debe definirse en la etapa 01; no improvisarlo durante la implementación.
- No migrar copias de `gestor_modulos` de otros esquemas sin evidencia de uso.
- Respetar diferencias de nombres, casing, nulabilidad y longitudes de las tablas de usuario por módulo.
- Un correo ausente o inválido con 2FA requerido debe fallar de forma controlada, nunca degradar silenciosamente a un factor.
- La recuperación de contraseña actual debe seguir funcionando y no reutilizar challenges de login.

## PRUEBAS MINIMAS TRANSVERSALES

- Login actual con 2FA desactivado en los cuatro módulos.
- Credenciales inválidas, usuario bloqueado/inactivo y empresa/módulo inválidos.
- 2FA activo: OTP correcto, incorrecto, expirado, consumido, máximo de intentos y reenvío.
- Ausencia de correo, fallo de envío, refresh/back, doble envío y consumo concurrente.
- Aislamiento entre usuario, módulo, empresa, propósito y sesión WebForms.
- Ausencia de cookie/sesión/auditoría de éxito antes del OTP.
- Permisos, redirección y sesión equivalentes al comportamiento previo después del OTP.
- Recuperación de contraseña sin regresión.

## DOCUMENTACION Y ENTREGA

Actualizar la exploración si la evidencia del código contradice una decisión. Mantener una única carpeta documental de implementación bajo `Doc/Actualizacion/Login/Implementacion/`; no crear una carpeta distinta por etapa.

Cada entrega debe informar: cambios, archivos, pruebas y compilación con comandos/resultados, compatibilidad verificada, documentación y riesgos pendientes. No afirmar que una prueba pasó si no pudo ejecutarse.

## Correcciones opsxj:prompt-review

Estas reglas proceden de `opsxj:prompt-review` y fueron ajustadas a ASP.NET Web Forms, VB.NET, MySQL y la infraestructura real de este repositorio.

## Rol esperado
Aplicar el rol técnico definido por esta etapa y detenerse si el código real contradice sus límites.

## Objetivo
Completar únicamente el objetivo verificable de esta etapa sin adelantar implementación, activación o pruebas de etapas posteriores.

## Restricciones criticas
- No introducir cambios fuera del alcance de Login 2FA declarado en esta carpeta.
- Preservar el login, recuperación de contraseña, contratos públicos y comportamiento de los cuatro módulos cuando 2FA esté desactivado.

## Criterios de aceptacion
- Se cumplen los criterios específicos de la etapa, las pruebas anteriores afectadas continúan pasando y no existe regresión conocida con 2FA desactivado.

## Contexto obligatorio
Leer `00-contexto-obligatorio.md`, la exploración, los entregables de etapas previas y el código vigente de `gestor.aspx`, `Defaul`, `Modelo`, `DTOs`, `Services`, `Infrastructure`, `webservice`, proyecto y pruebas que resulten afectados.

## Pruebas obligatorias
Ejecutar pruebas focales con la infraestructura existente, `msbuild .\\GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug` cuando esté disponible y Playwright solo en las etapas y ambientes expresamente autorizados; registrar comandos, códigos de salida y resultados reales.

## Documentacion tecnica
Actualizar únicamente el paquete canónico `Doc/Actualizacion/Login/Implementacion/` y la exploración cuando cambie una decisión comprobada.

## Entregable final
Entregar archivos modificados, pruebas y compilación ejecutadas, documentación, evidencia sanitizada, limitaciones y riesgos coherentes con lo realmente implementado en la etapa.

## Requisitos positivos
- Implementar el comportamiento esperado con contratos tipados y responsabilidades claras.
- Mantener la integracion sobre los puntos de extension existentes del repo.
- Dejar evidencia de pruebas y documentacion tecnica actualizada.

Mantener documentado el flujo exacto Credenciales → Preautenticación → Challenge → Correo → ASMX → Verificación atómica → Finalización legacy, incluidos errores y alternativas por módulo.

Exigir MSBuild del proyecto WebForms afectado y registrar comando, código de salida y errores; si MSBuild no está disponible, documentar el bloqueo y una verificación reproducible sin afirmar éxito.

Usar las pruebas existentes del repositorio y `node --test` para contratos CJS cuando corresponda; no introducir Vitest, Testing Library ni otro runner sin necesidad técnica aprobada.

Declarar que código + E2E + validación autorizada + evidencia saneada son una única unidad de entrega dentro del mismo cambio; no crear una tarea o entrega E2E independiente.

Reutilizar exclusivamente `tools/e2e`, su autenticación, configuración, validadores, evidencias y utilidades; prohibir login, arnés, proyecto Playwright, configuración o `.env` paralelos.

Antes de autenticar, exigir lectura de `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos/tareas descartables expresamente autorizados.

Exigir secretos efímeros, prohibir exponer/imprimir/persistir credenciales, cookies, tokens y cadenas de conexión, usar verificaciones solo `SELECT` y conservar evidencia saneada.

Exigir cobertura E2E, cuando aplique, de autorización/control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada.

Respetar feature flags, gates, usuarios, grupos y seguridad sin habilitarlos arbitrariamente; no cerrar sin validación autorizada, registrar bloqueo explícito y prohibir mocks, simulaciones, resultados inventados y evidencia ficticia.
