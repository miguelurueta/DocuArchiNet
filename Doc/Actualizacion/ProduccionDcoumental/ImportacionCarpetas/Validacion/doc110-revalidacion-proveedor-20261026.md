# DOC-110: revalidación de proveedor en publicación y selección

Estado vigente: **aceptación HTTP de la ampliación aprobada**, 1 passed (23.0 s), caso 22.0 s. Resultado aportado por el usuario después de la repetición autorizada; detalle al final. Las anotaciones NO_EJECUTADO/FALLIDO siguientes se conservan como historial previo.

Fecha: 2026-10-26. Base Git: `b2e93b726f221868cf2b82d43200a8e568871117`, con cambios locales sin commit. Avance de 14.11/14.29; no cierra aceptación integrada.

## Entregado

- `tests/PerfilImportacionServiceBehaviorTests.cs`: regresión del servicio real al publicar y seleccionar con referencia persistida a proveedor BORRADOR. Rechazo CONFIGURACION_INVALIDA, documento completo inalterado y ninguna escritura ni consulta a filas de negocio. También prueba ACCESO_DENEGADO cuando el repositorio deniega la referencia no disponible y selección administrativa positiva cuando vuelve a estar publicada/disponible. El repositorio y los catálogos son dobles explícitos; no acredita MySQL ni HTTP.
- `tools/e2e/tests/support/doc110-provider-revalidation.cjs`: integrado en `--provider-profile` después del guardado visual y antes de la publicación. Lee revisión vigente del proveedor descartable (crear su tercera versión incrementó cabecera), desactiva, exige PublicarVersion 403 ACCESO_DENEGADO sin datos y comprueba perfil completo e historial intactos. Restaura disponibilidad en finally leyendo estado/revisión; si se pierde la respuesta de desactivación, no repite esa escritura. El finally exterior del ciclo conserva la desactivación final del proveedor descartable.
- `tools/e2e/tests/doc110-provider-revalidation.test.cjs`: dobles de transporte comprueban recorrido, ambas revisiones requeridas, rechazo por motivo incorrecto, modificación del perfil/historial y recuperación ante respuesta perdida. No prueban IIS ni transacciones reales.

## Validación ejecutada

```powershell
node --test tests/doc110-perfil-importacion-behavior.test.cjs tools/e2e/tests/doc110-provider-revalidation.test.cjs tools/e2e/tests/doc110-provider-draft-consumer.test.cjs tools/e2e/tests/doc110-provider-history.test.cjs
```

Resultado: **8 PASS, 0 FAIL**. Incluye compilación vbc/csc y ejecución de componentes en .NET Framework mediante el arnés existente; no es una compilación completa del proyecto WebForms.

`node --check` PASS para runner de sesión, ciclo de publicación, soporte nuevo y prueba nueva. `git diff --check` PASS; aviso de normalización LF/CRLF en informe preexistente de opsxj. Cambios de esta continuación limitados a pruebas, soporte E2E, aviso del runner y documentación; sin cambios productivos VB, SII, SQL ni despliegue.

## Aceptación pendiente

Comando preparado para consola interactiva, tras autorización expresa de ambiente/cuenta y datos descartables:

```powershell
npm.cmd --prefix tools/e2e run test:doc110:session -- --provider-profile --headed
```

Reutiliza perfil runtime, login, secretos efímeros, runner y reporteros existentes. Requiere permisos de perfiles/proveedores, fuente DA autorizada y CONSECUTIVO unívoco introducido por consola sin imprimirlo. Activa/desactiva únicamente el proveedor creado por la corrida, publica dos versiones del perfil y conserva el proveedor finalmente inactivo. No modifica fuente DA ni crea expedientes/documentos.

Estado de esta ampliación HTTP: **NO_EJECUTADO**, sin autorización específica nueva en esta sesión. Los PASS autenticados de versiones anteriores del recorrido no cubren estas aserciones nuevas. La selección con un proveedor en borrador queda comprobada localmente por el servicio; no se fuerza un perfil inválido en MySQL para simularla por HTTP. Las tareas 14.11/14.29 permanecen abiertas con sus demás criterios y pruebas reales pendientes.

## Corrida autorizada posterior

El usuario autorizó la ejecución y aportó resultado de `--provider-profile --headed`: **1 failed, 26.6 s**. Pasan autocompletado, metadatos autorizados, alta/configuración/guardado/validación visual, consultas ausente y ambigua, ensayo sin mutación, publicación/edición rechazada y segunda versión conservando la primera. El ensayo de la clave ingresada retorna AMBIGUO donde el recorrido positivo exige OK. No se registra el valor de la clave.

La corrida falla por precondición de datos: necesita CONSECUTIVO con una sola coincidencia. Se detiene antes de activar el proveedor y antes del recorrido de perfil/revalidación; según el flujo ejecutado, el proveedor descartable permanece inactivo. Las nuevas aserciones de revalidación HTTP siguen **NO_EJECUTADO**. No acredita fallo del rechazo de publicación ni selección; no se cambia el producto para elegir una fila arbitraria.

Se añade diagnóstico explícito y saneado para clave ambigua/ausente. Sintaxis `node --check tools/e2e/tests/support/doc110-provider-lifecycle.cjs` PASS. Repetición pendiente con clave autorizada unívoca; la autorización del recorrido se conserva para ese mismo ambiente/cuenta y alcance.

## Repetición autorizada: PASS

Resultado aportado por el usuario: **1 passed (23.0 s)**, caso **22.0 s**. El reporte local `tools/e2e/artifacts/doc110-publication-result.json` registra PASS y finalización `2026-10-26T21:38:56.239Z`; los marcadores de los soportes adicionales proceden de la salida saneada compartida por el usuario. Misma base Git con cambios locales; no representa un commit nuevo ni un despliegue del agente.

- Clave unívoca confirmada; proveedor creado/configurado/guardado/validado/publicado desde UI y primera versión conservada tras segunda publicación.
- Proveedor activo con tercera versión BORRADOR: excluido del catálogo consumidor; GuardarBorrador y EnsayarReglas con ID directo rechazados, perfil y proveedor inalterados.
- `DISABLED_PROVIDER_REVALIDATED_BEFORE_PUBLICATION_PROFILE_AND_HISTORY_UNCHANGED`: tras desactivar el proveedor descartable, PublicarVersion rechaza 403 ACCESO_DENEGADO sin datos; lectura posterior verifica perfil completo e historial intactos.
- `DISPOSABLE_PROVIDER_RESTORED_FOR_PUBLICATION_LIFECYCLE`: restauración con revisión vigente, contrato y revisión de versión preservados. El ensayo posterior resuelve el índice desde la fuente real y la publicación continúa.
- Dos versiones del perfil publicadas con EXPEDIENTE/LOTE, edición publicada rechazada y primera publicación inmutable.
- Perfil y proveedor descartables finalmente desactivados. Ambas publicaciones y sus referencias al proveedor, así como el historial, permanecen intactos; proveedor inactivo excluido del catálogo consumidor.

Esta repetición supera el fallo de precondición anterior y el NO_EJECUTADO de revalidación de disponibilidad al publicar. No acredita por HTTP un perfil persistido con referencia a proveedor BORRADOR: guardar ya lo rechaza; la defensa de publicación/selección sobre ese snapshot se verifica en el arnés .NET. No acredita todos los criterios restantes de 14.11/14.29, aislamiento empresarial, revocación externa ni aceptación del catálogo de fuentes. Sin controles SELECT adicionales en esta sesión; persistencia observada mediante lecturas ASMX. No se consultaron ni registraron claves de negocio, contraseñas, cookies o tokens en esta evidencia.
