## 1. Consolidación del host y configuración

- [x] 1.1 Convertir el `HtmlMeta` viewport de `Defaul/WebFormInicioDocuarchiGestion.aspx` en un elemento estático visible y retirar del code-behind las constantes, lectura de configuración, evaluación de perfil y `PreRender` que solo controlan su visibilidad.
- [x] 1.2 Eliminar `WorkflowCentroTrabajoModernEnabled` y `WorkflowCentroTrabajoModernPilotProfiles` de `Web.config` y comprobar que no quedan referencias de producción, sin cambiar `WorkflowCentroTrabajoModernActive`, modo oficial, usuarios, grupos ni metadatos del gate operativo.

## 2. Pruebas de no regresión

- [x] 2.1 Extender `tests/workflow-modern-feature-gate.test.cjs` para comprobar que el host conserva un viewport estático, no lee perfil/sesión ni gate operativo, y que `Webworkflow.aspx` conserva su bootstrap y sus recursos modernos condicionados por el gate único.
- [x] 2.2 Corregir las aserciones de configuración de la suite para validar el estado fail-closed del repositorio y la ausencia de las dos claves retiradas, sin asumir una activación de ambiente.

## 3. Verificación y evidencia

- [x] 3.1 Ejecutar la suite focal de gate y envío a grupo, buscar referencias residuales de las claves históricas y compilar el proyecto disponible; registrar comandos y resultados en el cambio.
- [x] 3.2 Durante desarrollo y validación no autorizada, mantener `WorkflowCentroTrabajoModernActive=false` y usuarios/grupos vacíos al terminar; no ejecutar E2E autenticada, carga ni cambiar gates sin autorización explícita de ambiente y cuentas.

## 4. Separación de presentación y operaciones

- [x] 4.1 Incorporar una señal de presentación constante en `Webworkflow.aspx.vb`; usarla para viewport, clase raíz, recursos visuales y ramas de marcado DOC-2, sin cambiar el valor ni la evaluación de `WorkflowCentroTrabajoModernActive`.
- [x] 4.2 Dividir el registro de activos visuales y bootstraps de operación: cargar los primeros de forma constante y conservar bajo el gate los scripts e inicializadores de preview, búsqueda, confirmación y ejecución ASMX.
- [x] 4.3 Conservar las rutas de postback legacy para `Continuar flujo` y `Enviar a grupo` cuando el gate esté inactivo, aunque los controles se presenten con el estilo moderno; no marcar esos disparadores como ASMX activos fuera de alcance.

## 5. Pruebas y cierre seguro

- [x] 5.1 Extender la prueba focal para probar presentación constante, ausencia de bootstrap operativo fuera del gate y respaldo legacy de los disparadores; ejecutar las suites unitarias relacionadas y compilar el proyecto.
- [x] 5.2 Comprobar y registrar que la configuración queda `WorkflowCentroTrabajoModernActive=false`, con usuarios y grupos vacíos; no ejecutar E2E autenticada, carga ni cambiar gates sin autorización explícita de ambiente y cuentas.

## 6. Retiro de accesos legacy visibles

- [x] 6.1 Sustituir los accesos de `Continuar flujo` y `Enviar a grupo` por botones modernos únicos, sin `onclick` ni postback legacy, habilitados solo por la decisión de operación ya existente.
- [x] 6.2 Actualizar la regresión focal para prohibir los disparadores legacy en ambos controles y verificar el estado deshabilitado fail-closed; ejecutar pruebas focales, compilación y comprobación final del gate sin E2E.

## 7. Entrega de activos visuales actualizados

- [x] 7.1 Incrementar las versiones de URL del CSS y adaptador visual del centro de trabajo, y fijar con prueba focal que el markup entrega las versiones nuevas junto con los selectores por `id` de los dos controles modernos.
- [x] 7.2 Ejecutar la suite focal, comprobar formato y registrar que la corrección no cambió el gate ni ejecutó E2E.

## 8. Política oficial global

- [x] 8.1 Convertir el evaluador central de feature gate en una política oficial para todo contexto Workflow válido, sin lectura de configuración, alcance de usuario/grupo, piloto, exclusiones ni rollback; mantener los bloqueos de sesión y negocio fuera de esa política.
- [x] 8.2 Habilitar los dos controles modernos y sus bootstraps para contextos válidos, y adaptar las pruebas de transición y grupo para comprobar la ausencia de restricción de despliegue y la conservación de validaciones de negocio.
- [x] 8.3 Ejecutar la suite focal, compilación, comprobación de configuración sin modificar y validación OpenSpec; no ejecutar E2E ni carga.
