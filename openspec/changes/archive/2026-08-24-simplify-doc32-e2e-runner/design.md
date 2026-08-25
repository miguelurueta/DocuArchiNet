## Context

Los iniciadores E2E actuales solicitan la misma configuración para cada modo y cada DOC. DOC-32 divide preview, ejecución y concurrencia entre tres procesos, aunque las tres etapas necesitan el mismo ambiente, sesión Gestión y controles de solo lectura. Los secretos solo pueden vivir durante una corrida y las operaciones mutantes requieren autorización explícita; véanse `proposal.md` y las especificaciones de este cambio.

## Goals / Non-Goals

**Goals:**

- Ejecutar una secuencia E2E completa mediante un solo comando, perfil no sensible y una sesión efímera de secretos.
- Permitir perfiles distintos por DOC y ambiente sin permitir que un perfil ejecute comandos arbitrarios ni altere la política del DOC.
- Permitir que Codex inicie una única secuencia en una TTY y que el operador autorizado introduzca los secretos una sola vez, sin recibirlos ni imprimirlos en la conversación.
- Mantener los comandos específicos existentes durante la migración.

**Non-Goals:**

- Persistir secretos en archivos, variables de usuario, evidencia, Git o historial de comandos.
- Omitir autorizaciones de ambiente, ejecución o concurrencia.
- Cambiar endpoints Workflow, habilitar el gate o crear logins E2E alternativos.
- Convertir la carrera fija de cada DOC en una prueba de carga configurable.

## Decisions

### D-01 — Un orquestador registrado, no scripts elegidos desde perfiles

Se añadirá un iniciador común `workflow-e2e-runner.cjs` y un registro de DOC confiable. El registro define para cada DOC los campos no sensibles admisibles, las etapas ordenadas, los comandos hijos existentes, las autorizaciones requeridas y las variables de entorno que consume cada etapa. DOC-32 será el primer registro y conservará preview → ejecución → concurrencia.

El perfil es un JSON leído desde una ruta explícita, por lo que `C:\cert\contet.txt` será válido si contiene JSON. Incluye `doc`, `environment`, URL base, módulo, DSN ODBC no sensible, identificadores de tareas descartables, la lista exacta de actividades esperadas en el preview, los nombres exactos de actividad elegida para ejecución y concurrencia, y el nombre de la actividad activa final esperada para ejecución, consultas `SELECT` de un parámetro y presupuestos. Si el preview presenta varias actividades, el harness solicita la página máxima acotada de 50 resultados, exige que la lista recibida coincida sin repetidos y sin páginas adicionales con la lista configurada, busca una coincidencia única del nombre de selección y falla cerrada antes del endpoint mutante en vez de depender del orden de resultados. La carrera aplica la misma selección explícita contra su propio preview vigente y no usa el primer resultado. Tras una respuesta de ejecución exitosa, un control ODBC fijo y de solo lectura verifica que la actividad activa final coincide con el nombre final configurado; ambos nombres pueden diferir cuando una asignación de destinatario resuelve otra actividad efectiva. El control devuelve únicamente coincidencia, discrepancia o ambigüedad. El esquema se validará con lista estricta de claves; campos desconocidos y claves o valores sensibles se rechazarán sin mostrar su contenido. Un perfil no podrá proporcionar comandos, rutas de scripts, banderas de autorización, cookies, tokens, cuentas, contraseñas ni cadenas de conexión.

Se descarta reutilizar un `.env` o pasar comandos desde el perfil: ambos permitirían persistencia de secretos o ejecución no revisada.

### D-02 — Captura única de secretos en una consola interactiva

El orquestador exigirá una TTY antes de solicitar secretos. Reutilizará el adaptador de consola existente para solicitar una sola vez cuenta Workflow, contraseña oculta y usuario/contraseña MySQL de solo lectura; el DSN ODBC proviene solo del perfil no sensible. Después los mapeará en memoria a las variables efímeras esperadas por el DOC. Nunca los incorporará a argumentos, mensajes, archivos, variables de usuario ni evidencia.

Si no existe TTY o el operador no completa un valor, la corrida fallará cerrada antes de abrir navegador, sesión o conexión MySQL. Los valores se conservarán únicamente en el entorno aislado de los procesos hijos y se eliminarán en todos los caminos de salida.

Se descartan perfiles cifrados, Administrador de credenciales y otros almacenes locales: todos persistirían credenciales o cadenas de conexión, lo que contradice la política de esta capacidad.

### D-03 — Una invocación con autorizaciones explícitas no persistentes

El nuevo contrato será equivalente a:

```powershell
npm.cmd --prefix tools/e2e run test:workflow:run -- --doc doc32 --profile C:\cert\contet.txt --authorize environment,execution,concurrency
```

`--authorize` no se puede definir en el perfil y se valida contra las etapas solicitadas. En uso asistido por Codex, la autorización conversacional del responsable precede a la invocación; en uso manual, los flags constituyen la confirmación explícita de esa corrida. La ausencia de una autorización requerida impide recuperar secretos o iniciar la etapa asociada.

El orquestador validará primero perfil, contrato del DOC, autorizaciones, presupuestos, consultas `SELECT` y el gate local. Después exigirá la TTY, capturará una sola vez los secretos, construirá un entorno hijo aislado y ejecutará las etapas en orden. Si una etapa falla, excede su presupuesto o no produce evidencia válida, no se ejecutarán etapas posteriores. Un bloque `finally` elimina las variables efímeras de ese entorno y repite los controles de cierre.

Se descarta reutilizar una autorización guardada en el perfil o un flag global: impediría distinguir la autorización de preview, ejecución y concurrencia de cada corrida.

### D-04 — Adaptadores finos para conservar las suites por DOC

Los scripts y Playwright existentes seguirán siendo propietarios de sus aserciones, contratos ASMX, consultas y evidencia. El registro solo adaptará los valores validados a las variables que ya consumen y decidirá cuál script lanzar. DOC-32 reutilizará su preview, ejecución y carrera actuales; las migraciones de otros DOC añadirán un registro y pruebas de política sin cambiar su semántica E2E.

Se añadirán pruebas unitarias del lector de perfiles, del validador de claves, del mapeo del registro, del rechazo de autorizaciones faltantes y del recolector interactivo simulado. Las pruebas de política verificarán que ningún perfil, registro, salida ni artefacto admita secretos o cadenas de conexión. La E2E real continuará requiriendo autorización explícita y no se ejecutará en pruebas automáticas.

## Risks / Trade-offs

- [Un perfil erróneo puede apuntar al DOC o ambiente incorrecto] → El registro exige coincidencia de DOC, lista estricta de claves y validación previa de URL, tareas, SQL y presupuestos.
- [La consola de Codex no dispone de TTY o de un operador local] → El orquestador falla cerrado sin iniciar E2E y el operador ejecuta el único comando desde una consola interactiva autorizada.
- [Una autorización en la línea de comandos puede permanecer en historial] → Solo contiene nombres de etapas, nunca secretos; cada ejecución exige flags explícitos y no los lee del perfil.
- [La secuencia interrumpe después de una etapa mutante] → El `finally` conserva evidencia saneada, comprueba gate y rutas legacy, y no intenta revertir una transición confirmada.

## Migration Plan

1. Incorporar el orquestador, el lector de perfiles, el recolector interactivo único y el registro DOC-32 detrás de un comando nuevo.
2. Añadir una plantilla de perfil sin valores reales y documentar la captura única de secretos durante cada corrida.
3. Validar con pruebas locales de contrato y política; ejecutar una E2E DOC-32 solo con la autorización explícita del responsable.
4. Conservar `test:doc32:preview`, `test:doc32:execute` y `test:doc32:concurrency` mientras se migren otros DOC.
5. Para rollback, eliminar el uso del nuevo comando y volver a los comandos específicos; no se tocan datos Workflow, gate ni archivos legacy.
