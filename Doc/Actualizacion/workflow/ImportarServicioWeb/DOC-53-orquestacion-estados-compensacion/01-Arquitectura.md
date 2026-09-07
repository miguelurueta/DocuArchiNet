# Arquitectura

- Ticket: DOC-53
- Cambio OpenSpec: doc-53-orquestacion-estados
- Clasificacion: cross_cutting

`ImportServiceOrchestrator` es el coordinador moderno único. Revalida el contexto, lee la intención DOC-52, compara su versión y procesa cada elemento y fase de manera estrictamente secuencial. Los efectos se reciben como `IImportExecutionStep`; la máquina de estados y la fábrica de resultados permanecen puras y sin dependencias WebForms.

`LegacyImportDocumentStorageAdapter` es la única frontera nueva que conoce `ClassAlmacenamiento`. No se modifican ASMX, JavaScript, Integracionccv, ServiciosIntegracion ni almacenamiento legacy. La implementación coexiste con el recorrido vigente.
