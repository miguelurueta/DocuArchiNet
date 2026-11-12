# DOC-119 — filtros de seguimiento: reproducción local

Base: `94ca9277ce234b337b73bdda9bbd881e037b3047`, carpeta principal y rama feature/DOC-119. El cliente productivo seguía sin cambios al reproducir; las pruebas nuevas se añadieron al archivo existente `tests/doc111-importacion-carpetas-client.test.cjs`.

## Reproducción de 2.1

Comando: `node --test --test-name-pattern="DOC-119:" tests/doc111-importacion-carpetas-client.test.cjs`.

Resultado inicial: 2 pruebas ejecutadas, 0 PASS, 2 FAIL. Ambas se detienen al comprobar que falta el control de referencia de carpeta: se esperaba un filtro y se encontraron cero. La vista existente no ofrece filtros de raíz/efecto/recepción; el controlador tampoco los transporta en consulta ni CSV, aunque `ServicioConsultaImportacion` y su repositorio ya los admiten.

El caso usa el cliente y vista reales en Chromium, datos/servicios sintéticos con cinco archivos y estados mixtos, dos carpetas homónimas y límite de dos filas por página. Registra únicamente solicitudes sintéticas de consulta/CSV y rechaza cualquier otra operación. No ejecuta autenticación, IIS, MySQL, recepción, almacenamiento ni carga reales. No acredita SEG-01/02 integrados.

Corrección autorizada por D-02/RQ-02: presentar y transportar los filtros existentes, conservarlos al paginar y exportar la consulta de la página representada. Una consulta filtrada rechazada no debe dejar visible una exportación de resultados anteriores.

## Corrección y verificación de 2.2/2.3

La vista ofrece filtros de estado de carpeta y de referencia de carpeta/efecto/recepción de archivos. Aplicar o limpiar filtros reinicia su página; paginar conserva los filtros activos. Los rótulos accesibles diferencian los controles y se verifica presentación a 390 px.

El controlador construye solicitudes con los campos DTO existentes. La exportación copia la solicitud que produjo la página de archivos representada, conservando lote/tipo/filtros/página/tamaño. Consultar invalida los resultados anteriores, de modo que un rechazo no ofrece exportar una página obsoleta. No se cambia servicio, repositorio, autoridad ni esquema. Se actualizan las versiones de recursos modificados en el ASPX para renovar caché.

Comando de regresión: `node --test tests/doc111-importacion-carpetas-client.test.cjs tests/doc110-perfil-importacion-behavior.test.cjs`.

Resultado: 20/20 pruebas Node PASS, cero fallos/omitidas; incluye 18 del cliente y dos del arnés backend/DDL. El contenedor Framework ejecuta también sus casos internos, sin sumarlos otra vez. Las dos nuevas pruebas DOC-119 pasan: filtrar → paginar → descargar una página CSV por Blob → limpiar → filtrar carpetas; y denegación sin filas/exportación anteriores. Se conservan pruebas previas de recarga, ramas, autoridad, CSV/CeldaCsv y transporte sin replay.

La descarga nueva verifica selección/página y transporte usando contenido CSV sintético; la neutralización productiva de fórmulas pertenece al arnés backend existente. Esto no acredita descarga desde IIS ni SEG-01/02 reales. No se reorganizaron accesos ni se cruzó la compuerta 2.7.

Después del ajuste de Enter para aplicar referencia sin submit del formulario WebForms se reejecutó únicamente el archivo cliente: 18/18 PASS. El caso principal usa un formulario y comprueba cero submits; el caso de denegación conserva aplicación por botón. No se suman estas corridas como pruebas adicionales ni se repite el backend sin cambios.


## Huellas de fuentes del corte verificado

- Gestion/WebFormProducionDocumental.aspx: 2fd3ca1a0a6408d6b22da3d1acffd9fe8474808843ccce409047ea498d915c8e
- js/gestion/ImportacionCarpetas/ImportacionCarpetas.js: 23922f181b9fec5def299048df8e5c7da5e26bce883580fd80e2033775fbf58e
- js/gestion/ImportacionCarpetas/ImportacionCarpetasVista.js: 51491bcb439fd0b5721166df33bf027e7bebc894f1a11e6cb6d022c10bfdb3a8
- js/gestion/ImportacionCarpetas/ImportacionCarpetas.css: 2c79fa2e67495da55644a8e66206fe41c89e9744b6ba00f7556abc5291a6689c
- tests/doc111-importacion-carpetas-client.test.cjs: 87eef9e85a1e7bb79a537fc6ae5fb6881eb4404f52ed7d35f29ad72fc4de20d6
