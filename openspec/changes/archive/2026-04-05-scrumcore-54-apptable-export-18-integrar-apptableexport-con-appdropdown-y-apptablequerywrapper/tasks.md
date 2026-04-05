## 1. Integracion de exportacion en la banda operativa de tabla

- [x] 1.1 Ajustar `AppTableQueryWrapper` para que `paginationActions` sea el punto canonico de acciones operativas ligadas a la tabla, manteniendo exportacion en la misma banda visual que rango, page size y navegacion.
- [x] 1.2 Revisar la composicion consumidora actual para montar `AppTableExport` desde `paginationActions` y evitar uso de `headerActions` o toolbars separadas para la descarga.

## 2. Consolidacion del patron AppDropdown + AppTableExport

- [x] 2.1 Validar que `AppTableExport` siga delegando el menu a `AppDropdown`, agrupando acciones por formato y exponiendo solo modos realmente soportados por el datasource activo.
- [x] 2.2 Ajustar etiquetas, disabled states y semantica visible de formatos o modos no ejecutables para que el trigger de descarga comunique con claridad el alcance disponible.

## 3. Estados no destructivos de exportacion

- [x] 3.1 Separar explicitamente el estado de descarga de cualquier `loading` de datos de tabla para que exportar no active skeletons ni overlays sobre el contenido visible.
- [x] 3.2 Verificar que durante `exportLoading` solo se bloqueen el trigger y las opciones de descarga asociadas, preservando la tabla y el resto del contexto visual operativo.

## 4. Responsive y verificacion automatizada

- [x] 4.1 Ajustar estilos o estructura de la banda de controles para que exportacion permanezca dentro del mismo bloque responsive de `AppTableQueryWrapper` cuando el layout cambie de linea.
- [x] 4.2 Actualizar o agregar pruebas de `AppTableExport`, `AppTableQueryWrapper` y la pantalla consumidora para cubrir ubicacion en `paginationActions`, opciones segun capacidades reales y estado de exportacion no destructivo.
