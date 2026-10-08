# Prompts de implementación: doble factor de autenticación

Ejecutar los prompts en orden. En cada etapa se debe adjuntar primero `00-contexto-obligatorio.md` y después el prompt de la etapa. La fuente arquitectónica obligatoria es `../Exploracion/exploracion-doble-factor-autenticacion.md`.

| Orden | Archivo | Propósito |
|---|---|---|
| 0 | `00-contexto-obligatorio.md` | Alcance, compatibilidad y restricciones comunes. |
| 1 | `01-especificacion-y-linea-base.md` | Cerrar decisiones funcionales, contratos y línea base de no regresión. |
| 2 | `02-contratos-y-seguridad.md` | Diseñar contratos internos, estados y controles criptográficos. |
| 3 | `03-persistencia-y-concurrencia.md` | Versionar el esquema y construir persistencia atómica central. |
| 4 | `04-preautenticacion-y-adaptadores-modulo.md` | Separar preautenticación y resolver identidad para los cuatro módulos. |
| 5 | `05-envio-correo-existente.md` | Integrar el OTP con la función de correo ya existente. |
| 6 | `06-servicio-web-segundo-factor.md` | Exponer y probar la frontera ASMX productiva antes de construir la UI. |
| 7 | `07-interfaz-webforms-y-finalizacion.md` | Consumir el ASMX desde la UI y finalizar Forms Authentication sin regresión. |
| 8 | `08-pruebas-y-verificacion-transversal.md` | Verificar seguridad, módulos, compatibilidad y regresiones. |
| 9 | `09-e2e-autorizado.md` | Ejecutar E2E real únicamente con autorización explícita. |
| 10 | `10-liberacion-y-activacion-controlada.md` | Preparar despliegue, activación configurable y rollback. |

Reglas de secuencia:

- No iniciar una etapa si la anterior tiene decisiones bloqueantes o pruebas rojas.
- Cada etapa debe revisar el código real antes de editarlo; la exploración guía, pero no sustituye esa comprobación.
- Las etapas 1 a 8 no autorizan cambios de configuración, ejecución de migraciones en ambientes ni envío real de correo.
- Una prueba satisfactoria no autoriza activar 2FA.
- La implementación debe conservar el login actual cuando `RequiereSegundoFactor` esté desactivado.
- No ampliar en este incremento el alcance a recuperación de contraseña ni TOTP.

## Puertas de verificación incremental

No se debe esperar hasta el E2E final para detectar errores:

1. Etapas 01–02: pruebas locales de caracterización, contratos y seguridad.
2. Etapa 03: pruebas del repositorio, migración y concurrencia en infraestructura descartable autorizada.
3. Etapas 04–05: pruebas de aplicación por módulo con repositorio y correo simulados.
4. Etapa 06: pruebas directas del ASMX, primero anónimas y locales; la integración real se ejecuta únicamente con autorización.
5. Etapa 07: pruebas del cliente WebForms contra el mismo contrato ASMX ya verificado.
6. Etapa 08: regresión transversal completa.
7. Etapa 09: E2E del recorrido de usuario real.

Una falla se corrige dentro de la etapa que la introdujo y se vuelven a ejecutar sus pruebas y las anteriores afectadas. No se crea automáticamente un prompt separado para cada defecto.

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

## Reglas de ubicacion de codigo
- Ubicar modelos e interfaces en `Modelo/Login/SegundoFactor/`, DTOs en `DTOs/Login/SegundoFactor/`, aplicación en `Services/Login/SegundoFactor/`, persistencia/adaptadores en `Infrastructure/Login/SegundoFactor/` y la frontera HTTP en `webservice/`, siempre que la inspección de la etapa 01 confirme esas convenciones.
- Registrar todos los `.vb` y `.asmx` explícitamente en `GestionDocumental-Docuarchi.net.vbproj`.
- No crear una segunda aplicación, API, proyecto de pruebas o árbol `src/` ajeno a la estructura WebForms existente.

Preservar firmas y recorridos actuales no relacionados; prohibir endpoints alternos de credenciales, bypass del OTP, duplicación SMTP, SQL dentro del ASMX y cambios silenciosos de configuración para hacer pasar pruebas.

Exigir MSBuild del proyecto WebForms afectado y registrar comando, código de salida y errores; si MSBuild no está disponible, documentar el bloqueo y una verificación reproducible sin afirmar éxito.

Usar las pruebas existentes del repositorio y `node --test` para contratos CJS cuando corresponda; no introducir Vitest, Testing Library ni otro runner sin necesidad técnica aprobada.

Registrar comandos, códigos de salida, resultados y evidencia sanitizada en `Doc/Actualizacion/Login/Implementacion/04-pruebas-y-evidencia.md`.

Declarar que código + E2E + validación autorizada + evidencia saneada son una única unidad de entrega dentro del mismo cambio; no crear una tarea o entrega E2E independiente.

Reutilizar exclusivamente `tools/e2e`, su autenticación, configuración, validadores, evidencias y utilidades; prohibir login, arnés, proyecto Playwright, configuración o `.env` paralelos.

Antes de autenticar, exigir lectura de `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos/tareas descartables expresamente autorizados.

Exigir secretos efímeros, prohibir exponer/imprimir/persistir credenciales, cookies, tokens y cadenas de conexión, usar verificaciones solo `SELECT` y conservar evidencia saneada.

Exigir cobertura E2E, cuando aplique, de autorización/control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada.

Respetar feature flags, gates, usuarios, grupos y seguridad sin habilitarlos arbitrariamente; no cerrar sin validación autorizada, registrar bloqueo explícito y prohibir mocks, simulaciones, resultados inventados y evidencia ficticia.
