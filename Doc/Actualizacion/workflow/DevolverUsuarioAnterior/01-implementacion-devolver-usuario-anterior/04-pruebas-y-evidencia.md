# Pruebas y evidencia

DOC-36 incorpora pruebas focales y una matriz E2E protegida. La automatización E2E no habilita ni toca el ambiente por sí sola: cada etapa exige autorización explícita, solicita credenciales efímeras por TTY y usa un perfil JSON persistente sin secretos.

| Área | Evidencia requerida |
| --- | --- |
| Preview | Solo `SELECT` parametrizados; no escribe tarea, estado, auditoría, eventos ni negocio. |
| Historial | Último `id_Estado` anterior con usuario real (`Id_Usuario > 0`), snapshots grupales intermedios, ausencia, usuario retirado y Ruta/Flujo inconsistente. |
| Seguridad | Permiso específico, payload mínimo, auto-devolución con usuario autenticado y errores saneados. |
| Token | Malformado, vencido, snapshot cambiado y antecedente cambiado. |
| Concurrencia | Lock por tarea, tokens distintos, liberación y una sola mutación. |
| Motor | `Page = Nothing`, parámetros inhibidores de notificación/interfaz/eventos, éxito, rechazo y advertencia. |
| Aislamiento | Ninguna referencia a `Classgestionrespuesta`, `Verifica_respuesta_*` ni `Reasigna_respuesta_envia_tarea_usuario`. |
| Auditoría | Acción `ASMX_DEVOLVER_USUARIO_ANTERIOR`; falla de auditoría como advertencia posterior al éxito. |
| E2E preview | Sesión autenticada, endpoint `PreviewDevolverUsuarioAnterior`, token opaco y usuario/actividad mínimos; huellas ODBC de estado y auditoría idénticas antes/después. |
| E2E ejecución | Sobre una tarea descartable separada, preview vigente seguido exclusivamente de `{ idTarea, tokenVersion }`; una transición, auditoría y actividad final comprobadas por ODBC. |
| E2E concurrencia | Segunda tarea descartable y aislada: dos ejecuciones simultáneas con el mismo preview producen una sola transición; la otra queda bloqueada o en conflicto. |

La validación de implementación registrará comandos de pruebas focales y compilación disponibles, con resultados y limitaciones reproducibles. No se ejecutará E2E autenticada, carga ni una transición real en DOC-36.

## Evidencia DOC-36

| Control | Resultado |
| --- | --- |
| `node --test tests/workflow-return-user-previous.test.cjs tests/workflow-return-activity.test.cjs` | 25 pruebas aprobadas: DOC-36 y regresión DOC-32. |
| `node --test tests/workflow-return-user-previous.behavior.test.cjs tests/workflow-return-user-previous.test.cjs tests/workflow-return-activity.test.cjs` | 26 pruebas aprobadas: arnés conductual de preview, bloqueos, token, lock, adaptador, auditoría y preservación del usuario de flujo histórico, más regresión DOC-32. |
| `msbuild GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m:1 /v:minimal` | Compilación correcta; conserva advertencias históricas del proyecto. |
| `npm.cmd --prefix tools/e2e run test:workflow:runner` | 18 pruebas aprobadas: perfil DOC-36 saneado, autorizaciones por etapa, secreto efímero, recursos de ejecución/concurrencia distintos y comandos Playwright/Node correctos. |
| `node --test tools/e2e/tests/authenticated-workflow-session-usage.test.cjs` | La suite DOC-36 y su carrera reutilizan el helper de inicio de sesión autenticado, sin selectores duplicados. |
| `npm.cmd --prefix tools/e2e run test:doc36:policy` | 3 pruebas aprobadas: payload mínimo, endpoints exclusivos, huellas ODBC de solo lectura, gate local apagado, concurrencia de una mutación y perfil sin secretos. |
| Revisión estática | Sin referencias nuevas a respuestas, devolución legacy, controles Web Forms, `PuedeCambioRuta` ni escrituras SQL en el repositorio nuevo. |

## Gate E2E pendiente de autorización

La automatización está registrada en `tools/e2e/` y el ejemplo manual persistente es `tools/e2e/profiles/doc36-workflow-user-previous.profile.example.json`. Antes de una corrida autorizada, se copia a un archivo local editable fuera del control de versiones y se asignan únicamente los valores no sensibles: URL, DSN ODBC, identificadores de dos tareas descartables, actividades esperadas, consultas `SELECT` y presupuestos.

La única entrada para las tres etapas es:

```powershell
npm.cmd --prefix tools/e2e run test:workflow:run -- --doc doc36 --profile C:\ruta\perfil-doc36.json --authorize environment,execution,concurrency
```

El orquestador pide por TTY las cuentas temporalmente, no las persiste y elimina sus variables tras terminar. Para limitarse a preview se agrega `--stages preview` y se autoriza solo `environment`. Ejecución y concurrencia nunca se inician sin sus autorizaciones explícitas. Las tareas de ejecución y concurrencia deben ser distintas y estar preparadas como descartables; la reserva comprueba sus huellas ODBC de solo lectura antes de iniciarlas.

Estado de este cambio: **automatizado y validado localmente; no ejecutado en ambiente autenticado**. No se ejecutó carga, configuración de ambiente ni una tarea real durante DOC-36.

## Relevo

Al completar DOC-36, la etapa de interfaz consumirá únicamente estos dos endpoints y el token opaco. No podrá reabrir ni conservar el postback legado como ruta alternativa.

La matriz obligatoria de la etapa 03 debe ejecutar E2E de interfaz para: mostrar la confirmación con el contexto mínimo del preview; cancelar sin mutación; foco y teclado (`Escape`); impedir doble clic, cierre del modal o abandono mientras `EjecutarDevolverUsuarioAnterior` esté pendiente; restaurar la bandeja cuando termine; y consumir exclusivamente los endpoints DOC-36. DOC-36 no implementa esa interfaz ni activa rutas legacy.
