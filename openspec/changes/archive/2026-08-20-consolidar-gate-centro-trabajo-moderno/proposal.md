## Why

La página inicial aún conserva una activación histórica separada para el viewport responsive, mientras el centro de trabajo, los servicios ASMX y Enviar a grupo ya usan el gate operativo único. Esta duplicidad hace ambiguo qué configuración controla la experiencia moderna y mantiene un piloto obsoleto sin aportar autorización adicional.

## What Changes

- Retirar las claves históricas `WorkflowCentroTrabajoModernEnabled` y `WorkflowCentroTrabajoModernPilotProfiles`, junto con la decisión de servidor que solo las consume en la página inicial.
- Conservar de forma estática el `meta viewport` de la página inicial, para que eliminar la configuración no reduzca la experiencia responsive en teléfonos.
- Entregar de forma constante la estructura, estilos y comportamiento estrictamente visuales modernos de `Webworkflow.aspx`, incluso con el gate operativo inactivo.
- Sustituir la evaluación de `WorkflowCentroTrabajoModernActive`, usuarios, grupos, piloto y rollback por una política oficial única para todo contexto Workflow válido; las claves de gate locales se conservan sin efecto funcional para respetar la configuración de desarrollo existente.
- Retirar de los accesos visibles de `Continuar flujo` y `Enviar a grupo` toda ruta de postback o diálogo legacy: para cualquier contexto válido usan exclusivamente sus inicializadores ASMX modernos.
- Versionar los activos CSS y JavaScript de presentación al cambiar su lógica de ubicación o estilo, para que los navegadores no conserven un adaptador visual previo.
- **BREAKING**: las configuraciones externas que aún administren las dos claves históricas dejarán de tener efecto.

## Capabilities

### New Capabilities

<!-- Ninguna. -->

### Modified Capabilities

- `infraestructura-visual-aislada-reversible`: sustituir la dependencia del gate y perfil piloto históricos por un viewport estable y una única activación moderna para el centro de trabajo.

## Impact

- `Defaul/WebFormInicioDocuarchiGestion.aspx` y su code-behind, donde subsiste la decisión histórica.
- `Web.config`, para retirar las claves sin uso.
- Especificación y pruebas focales de la infraestructura visual y la página inicial, con revisión responsive manual posterior.
- No modifica los contratos ASMX ni las validaciones de sesión, permisos, requisitos, concurrencia y auditoría; retira la restricción de despliegue por gate y los accesos visibles a los flujos legacy de continuar y enviar a grupo.
