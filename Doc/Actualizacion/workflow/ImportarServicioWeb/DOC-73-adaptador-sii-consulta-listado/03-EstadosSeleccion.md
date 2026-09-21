# Estados y selección

El núcleo conserva `resolviendo-proveedor`, `consultando`, `vacio`, `resultados` y `error`. El adaptador traduce falta de autorización a `SII_NOT_AUTHORIZED` o al código seguro backend.

La lista admite Todos, Disponibles, Importados y Con novedad. Filtros, páginas y selección operan sobre la instantánea. Un item solo es seleccionable si no está `IMPORTED` y contiene acción `IMPORT` o `IMPORTAR`.

