# PROMPT ARQUITECTONICO  Ticket 05 FE
# Ajuste visual AppToolbar en tab Gestion

Rol esperado:
Arquitecto de software senior frontend (React, TypeScript estricto, componentes UI enterprise, consistencia visual y reutilizacion de componentes shared)


OBJETIVO

Ajustar visualmente el `AppToolbar` renderizado dentro del contenido del tab **Gestion** en `GestionRespuesta`, para unificar la presentacion de acciones y moverlas al lado izquierdo de la barra.


CONTEXTO EXISTENTE

- pagina orquestadora: `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx`
- contenido del primer tab: `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx`
- componente shared reutilizable: `AppToolbar`
- componente shared reutilizable para acciones: `AppButton`


ALCANCE

- modificar solo el toolbar interno del tab **Gestion**
- mantener la estructura general del workbench
- preservar el contrato del tab dentro de `GestionRespuesta`
- no afectar el tab **Documentos**


UBICACION (OBLIGATORIA)

```
src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/
```


REQUERIMIENTO VISUAL

El `AppToolbar` del tab **Gestion** debe mostrar exactamente tres botones alineados en la parte izquierda:

1. `Solicitud de Aprobacion` + icono
2. `Guardar` + icono
3. `Enviar` + icono


CONTRATO DE LOS BOTONES (OBLIGATORIO)

- todos los botones usan `AppButton`
- todos los botones usan `size="sm"`
- todos los botones usan `variant="ghost"`
- las tres acciones deben quedar visualmente agrupadas a la izquierda dentro del `AppToolbar`


RESTRICCIONES (OBLIGATORIAS)

- no introducir estilos globales
- no modificar el comportamiento del sistema de tabs
- no introducir logica de negocio nueva
- no usar `any`
- no reemplazar `AppToolbar` por otro contenedor
- no afectar el layout del editor principal ni la zona de adjuntos


REGLAS DE IMPLEMENTACION

1. revisar la configuracion actual de `actions`, `primaryAction` y cualquier otra accion inyectada al `AppToolbar`
2. normalizar las acciones para que las tres vivan en el bloque izquierdo del toolbar
3. eliminar jerarquia visual inconsistente entre acciones primarias y secundarias en este caso concreto
4. mantener consistencia con el lenguaje visual existente del modulo


ICONOGRAFIA

- reutilizar iconos ya disponibles en el proyecto para:
  - solicitud de aprobacion
  - guardar
  - enviar
- priorizar iconos de Ant Design si ya se usan en el componente


CRITERIOS DE ACEPTACION

- el tab **Gestion** renderiza tres acciones visibles en el `AppToolbar`
- las tres acciones quedan alineadas a la izquierda
- todas las acciones usan `AppButton` con `size="sm"` y `variant="ghost"`
- `Enviar` deja de verse como accion primaria destacada para este ajuste visual
- no hay regresiones visuales en el workbench del tab **Gestion**
- el tab **Documentos** no se ve afectado


PRUEBAS MINIMAS SUGERIDAS

- validar que el toolbar renderiza:
  - `Solicitud de Aprobacion`
  - `Guardar`
  - `Enviar`
- validar que ya no existe configuracion de accion primaria separada para este toolbar
- validar que el tab sigue renderizando correctamente el editor principal y la zona de adjuntos
