## 1. Diagnóstico y reproducción

- [ ] 1.1 Reproducir el error `Identifier 'useEffect' has already been declared` apuntando al archivo `GestionRespuestaMainTabContent.tsx` (lint/ts/build según pipeline).
- [ ] 1.2 Identificar y listar todos los imports duplicados (React y/o paquetes como `@ant-design/icons`) y cualquier regla de lint asociada que se dispare en la pantalla.

## 2. Corrección técnica (sin cambios funcionales)

- [ ] 2.1 Eliminar el import duplicado desde `"react"` y dejar un único import para los hooks requeridos.
- [ ] 2.2 Consolidar/ordenar imports duplicados adicionales (si el linter lo exige) sin alterar la lógica del componente.
- [ ] 2.3 Corregir errores de lint/TS desencadenados por el cambio (unused imports/vars, reglas de ordenamiento, tipado) manteniendo el comportamiento funcional.

## 3. Validación

- [ ] 3.1 Ejecutar el/los comandos de `lint` del proyecto y confirmar que la pantalla/archivo no reporta errores.
- [ ] 3.2 Ejecutar el/los comandos de `build` del proyecto y confirmar que compila sin errores relacionados.
- [ ] 3.3 Si existen tests aplicables a la pantalla, ejecutarlos y confirmar que pasan (o documentar si no aplica).

## 4. Evidencia y cierre

- [ ] 4.1 Registrar evidencia de ejecución (salida/resumen de `lint`/`build`/tests) en el change OpenSpec o comentario del PR.
- [ ] 4.2 Verificar que no hubo cambios funcionales (solo deduplicación/normalización de imports y ajustes para lint/build).

