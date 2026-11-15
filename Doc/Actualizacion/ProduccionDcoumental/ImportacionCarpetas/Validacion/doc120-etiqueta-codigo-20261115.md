# DOC-120 — etiqueta del expediente por código

Solicitud: «debe ser codigo unico». Base `99a0fd78879bbb092242fa8dbe7f7a8f160efcd1`. Clasificación LOCAL_DOC120; tareas3.69–3.71, D-04/RQ-04, D-05/RQ-05 y D-10/RQ-10.

Las nuevas creaciones guardan `comando.Codigo` en CODIGO_UNICO y ALEAS_EXPEDIENTE; `comando.Nombre` permanece en TEMA_EXPEDIENTE. La consulta añade codigoExpediente desde el único valor CODIGO no diferido de una raíz con identidad y estado confirmados; nombreExpediente conserva su significado informativo. Árbol y búsqueda presentan el código como texto, sin fallback al nombre/carpeta. Actualizar una relación visible conserva su identidad, selección, expansión y contador. Dos códigos iguales con IDs distintos mantienen dos nodos. Las relaciones siguen usando ID_EXPEDIENTE/id_registro.

La comprobación posterior a una nueva creación exige alias=código. Solo la verificación de un intento incierto con IDs conocidos admite también el alias histórico igual al nombre confirmado. Conserva comprobaciones estrictas de código, nombre, IDs, destino, autoridad, clasificación y demás campos; no busca por etiqueta ni reescribe evidencias. No se migran filas históricas ni se cambian políticas de coincidencia.

## Verificación local

| Comando o comprobación | Resultado | Evidencia local excluida de Git |
|---|---|---|
| `node --test --test-name-pattern=DOC120-TREE06 tests/doc120-importacion-arbol.test.cjs`, antes del cambio | Fallo reproducido: usa nombre y conserva etiqueta anterior | `.tmp/doc120-arbol-codigo-red.log` |
| `node --test tests/doc111-importacion-carpetas-client.test.cjs tests/doc120-importacion-arbol.test.cjs` |35/35| `.tmp/doc120-arbol-codigo-regresion.log` |
| `node --test tests/doc116-expedientes-contracts.test.cjs tests/doc116-internal-fault.test.cjs tests/doc119-navigation.test.cjs` |38/38| `.tmp/doc120-arbol-codigo-auxiliares.log` |
| `node --test tests/doc110-perfil-importacion-behavior.test.cjs` |2/2 wrappers Framework/DDL; casos internos no sumados| `.tmp/doc120-arbol-codigo-framework.log` |
| MSBuild del proyecto, `/t:Compile`, salida e intermedios aislados |0 errores; advertencias previas conservadas| `.tmp/doc120-arbol-codigo-build/output.log` |

Total **75/75 pruebas Node únicas**. Los7 casos del árbol cubren texto HTML seguro, código distinto del nombre, código ausente, dos códigos iguales, búsqueda/clic, expansor nativo System.Web, selección, expansión, rama oculta/ambigua, incertidumbre y fallo del insertor. Framework cubre código ausente/diferido/duplicado, identidad ausente, estado incierto y compatibilidad histórica sin mutar evidencia. El auxiliar DOC116 exige alias=código para futuras ejecuciones, con datos sintéticos de nombre/código distintos. Captura: `.tmp/doc120-arbol-codigo-20261115/primer-expediente.png`. Huellas LF de fuentes/logs en el JSON compañero. No se suman corridas repetidas ni cortes anteriores.

## Límites

Sin autenticación, SELECT de negocio, recepción, creación ni almacenamiento reales. Chromium usa servicios sintéticos y Framework usa dobles SQL/escritor. Compilación aislada: no se copia la DLL a bin ni se activa IIS. No acredita runtime instalado, concurrencia MySQL o aceptación integral. Los códigos repetidos siguen sujetos a la política actual; el caso de dos IDs prueba exclusivamente la proyección. Sin cambios de esquema, perfiles publicados, SII, almacenamiento compartido o limpieza.

Avance87/91 tareas,17 S/74 M,37 escenarios. Permanecen5.2/6.4/6.5/6.6 y aceptación integral abiertas.
