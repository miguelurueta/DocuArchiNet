# DOC-110: aceptación consolidada PER-01–03

8.6 completa con la corrida autenticada descrita a continuación. Las corridas históricas siguen siendo válidas para sus criterios, pero no adquieren retrospectivamente SHA o IDs que no registraron. Las secciones de preparación inferiores conservan el estado anterior a esta aceptación.

## Resultado confirmado

Salida aportada por el usuario: **1 passed (35.5 s)**, caso 34.2 s. Marcador PER_01_02_03_SHA_UI_HTTP_SELECT_CONFIRMED. Reporte consolidado PASS: inicio del helper 2026-10-27T16:43:11.681Z, fin 2026-10-27T16:43:22.157Z; esos tiempos no incluyen todo el login/búsqueda de la prueba.

Código registrado: HEAD b2e93b726f221868cf2b82d43200a8e568871117; SHA-256 agregado f9d4ed21c74bb89026818f04348e156c8b87db26d2a685878c371d8e2a08b95a para 13 archivos, incluida DLL activa. La huella se mantuvo igual al finalizar. HEAD no representa por sí solo el árbol sin commit.

| Caso | UI/HTTP real | SELECT del mismo fixture |
|---|---|---|
| PER-01 | Perfil 24: configuración UI, guardar ámbito vacío, PublicarVersion 400 AMBITO_REQUERIDO sin cambios; selección explícita EXPEDIENTE, publicación y relectura. | Versión 37, número 1 PUBLICADA con EXPEDIENTE; pertenencia al perfil y referencia publicada válidas. |
| PER-02 | Perfil 24: segunda versión 38 con LOTE; primera publicación íntegra e historial de dos. Al retirar permiso de la sesión vigente, GuardarBorrador y PublicarVersion dan 403 ACCESO_DENEGADO sin datos. Restauración recupera acceso y documentos/historial intactos. | Perfil inactivo; dos PUBLICADA numeradas 1/2 y ámbitos EXPEDIENTE/LOTE, 10 reglas totales, sin duplicados/huérfanos/referencias inválidas. |
| PER-03 | Perfil 23/versión 36: alta UI, reglas guardadas/releídas, dos separadores con ceros iniciales, publicación incompleta rechazada y revisión obsoleta sin cambios. | Perfil inactivo, una versión BORRADOR número 1 y una regla persistida. |

Reporte SQL doc110-acceptance-select-result.json PASS, MySQL 5.1.37-community: 16 comprobaciones (14 invariantes globales y dos formas exactas de fixture). Los IDs se contrastan contra los devueltos por las operaciones HTTP de esta misma corrida. No se atribuye a esta ejecución una carrera entre sesiones independientes: el ejercicio de borrador usa una sesión; esas carreras tienen evidencia separada de 10.19.

El permiso termina RESTORED a su valor original 1 y los demás campos conservan su huella. Una inspección SELECT independiente posterior confirmó nuevamente permiso concedido y fila inequívoca. Los perfiles 23/24 quedan desactivados; no se eliminaron ni se cargó otro PDF. No se modificaron fuentes o SII.

## Recorrido

Modo exclusivo --acceptance del runner DOC-110 existente, cuenta LUZ.AGUILERA y ambiente ya autorizados. Un login reutilizado; sin nuevos uploader/framework ni archivos de credenciales.

1. PER-03: ejercicio de borrador existente, alta UI, guardar/recargar reglas, dos separadores con ceros iniciales, publicación incompleta rechazada, historial y perfil desactivado.
2. PER-01: ejercicio de publicación existente sobre un segundo perfil. Añade guardado UI sin ámbito, rechazo HTTP de publicación con AMBITO_REQUERIDO y perfil intacto; restaura EXPEDIENTE desde UI, publica y relee.
3. PER-02: segunda versión LOTE, primera publicación inmutable e historial de dos publicaciones; perfil final desactivado. Reutiliza retirada/restauración puntual ya autorizada del permiso de LUZ.AGUILERA para exigir 403 en guardar y publicar, además de listado/detalle/historial. No atribuye el rechazo a sesión expirada.
4. SELECT: comprobador de identidades existente con IDs de ambos perfiles. Exige cabeceras inactivas, un BORRADOR con reglas en el perfil PER-03, dos PUBLICADA numeradas 1/2 con EXPEDIENTE/LOTE en PER-01/02 y reglas persistidas; también aplica los 14 invariantes globales. No modifica datos.

Los dos perfiles creados permanecen en BD como descartables desactivados; no se borran. No se toca facturas/créditos/egresos, no se cargan documentos y no se ejecutan lotes SII. El permiso temporal se restaura mediante el mecanismo y recuperación ya verificados.

## Código y reportes

Al inicio se registra HEAD y SHA-256 de 13 archivos seleccionados, incluidos servicios/repositorio/fachada/editor, driver/auxiliares de prueba, comprobador SELECT y DLL activa. Se calcula una huella agregada y se exige que no cambie durante el recorrido. HEAD por sí solo no representa el árbol sin commit; el manifiesto lo deja explícito. No se incluye Web.config, credenciales, tokens ni cuerpos de documentos.

Los reportes de borrador/publicación registran profileId y versionIds confirmados por servidor. El reporte consolidado doc110-acceptance-result.json enlaza estos IDs, manifiesto y resultado SQL. El SELECT se escribe en doc110-acceptance-select-result.json, separado del control global anterior. Las corridas antiguas no adquieren retrospectivamente esta información.

## Comprobaciones previas

Nueve pruebas Node PASS de revocación/fixture/historial y el arnés .NET PASS tras añadir el rechazo de ámbito requerido. Sintaxis de driver/orquestación PASS. La revocación local cubre ahora también PublicarVersion: cinco rechazos; las corridas antiguas de cuatro siguen documentadas como cuatro. Captura del manifiesto comprobada localmente.

La comprobación SQL se ejercitó con IDs inexistentes válidos sintácticamente: se detiene con FAIL en ACCEPTANCE_DRAFT, sin escrituras, en lugar de aceptar un reporte global como evidencia del caso. Esa ejecución negativa no es aceptación de los perfiles nuevos, que aún no existen.

```powershell
npm.cmd --prefix tools/e2e run test:doc110:session -- --acceptance --headed
```

Estado HTTP consolidado: **NO_EJECUTADO**. La preparación no cierra 8.6. Instalación nueva y operación sin esquema conservan sus propios pendientes de entorno aislado; no se retiran tablas del ambiente actual para fabricar esos casos.
