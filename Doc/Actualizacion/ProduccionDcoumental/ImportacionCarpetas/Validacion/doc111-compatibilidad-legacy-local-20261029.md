# DOC-111: compatibilidad previa a creación legacy, 2026-10-29

Avance parcial de 9.3, D-07/RQ-07. **95/130 tareas completas, sin incremento**. No se habilita creación o almacenamiento; falta completar el adaptador y la verificación de efectos.

Se amplían el comando y el catálogo compartidos existentes, sin nuevos archivos productivos ni cambios de contrato HTTP. PrepararDatosCreacionLegacy comprueba entradas, resuelve los nombres mediante la lectura autorizada existente y comprueba esos resultados. ValidarEntradasLegacy y ValidarDatosLegacy no modifican el comando, snapshot o nombres.

Se rechazan IDs que exceden Integer.MaxValue en los argumentos que pasan por la firma/helpers Integer de Conservación. Serie/subserie también llegan a argumentos Integer de las funciones de retención, en ClassGaExpediente:10663 y 10813. Se comprueba organigrama después de resolverlo. Los IDs propios del lote/intento no se convierten a Integer; siguen admitiendo el rango del contrato operativo.

El SQL legacy entrecomilla por concatenación sin una frontera demostrada para apóstrofos o barras inversas. El control rechaza esos caracteres, controles y surrogates en textos que se pasarían a ese SQL; no renombra, escapa ni recorta datos confirmados. Unicode BMP, espacios y comillas dobles se conservan. La lectura de catálogos por sí sola sigue admitiendo esos nombres: la restricción pertenece a la preparación de invocación legacy. No se presenta como garantía general de charset, XML o longitudes físicas.

Los datos de catálogo requieren el conjunto exacto de claves de la lectura existente, nombres obligatorios, organigrama válido y ausencias explícitas de subserie/responsable conformes al snapshot. La identificación del responsable conserva ceros iniciales y el límite de 60 de la firma.

Validación PASS:

- `node --test tests/doc110-perfil-importacion-behavior.test.cjs`: familias previas y nuevos casos Framework. Verifica textos exactos y ausencia de mutación, apóstrofos/barras/controles, rechazo de IDs Integer.MaxValue+1 antes de SQL, Integer.MaxValue aceptado, IDs operativos UInt32.MaxValue conservados, organigrama fuera de rango, datos faltantes y subserie inesperada. Las consultas del catálogo usan registros sintéticos; no se invocó creación.
- Build Debug completo sin despliegue, salida/intermedios `.tmp/doc111-compatibilidad-legacy-build/{bin,obj}`, log `.tmp/doc111-compatibilidad-legacy-build.log`, código 0.
- OpenSpec estricto y revisión de diferencias PASS. Diffs de ClassGaExpediente, ClassAlmacenamiento, ClassNeodynamic y conect vacíos.

Pendientes: ciclo de archivo (consulta al usuario del corte anterior aún sin elección concreta), composición final de argumentos/conexión RA, autoridad actual, barreras y comprobación/correlación del resultado. Continuar la implementación no se interpreta como elegir automáticamente un ciclo.

Sin SQL/HTTP/E2E reales, creación, almacenamiento, limpieza, cambios de configuración o despliegue en este corte. SII y las funciones legacy permanecen intactos.
