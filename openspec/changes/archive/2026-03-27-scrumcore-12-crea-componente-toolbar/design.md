## Context

El cambio `SCRUMCORE-12` nace desde Jira con el objetivo de crear un componente `AppToolbar` reutilizable para la SPA. El repo ya tiene un patron de componentes UI compartidos en `src/app/Components/UI` como `AppButton`, `AppInput`, `AppModal`, `AppTabs` y `AppDataTableMui`, pero no existe aun una pieza estandar para encabezados de modulo, acciones contextuales, filtros rapidos y composicion responsive de controles.

La necesidad es especialmente visible en modulos como `gestionCorrespondencia`, donde el layout actual define el contexto visual del modulo pero no existe una barra reutilizable para titulo, descripcion, breadcrumbs, acciones primarias/secundarias y zonas auxiliares. El componente debe integrarse con la arquitectura React 19 + TypeScript estricto + CSS Modules y convivir con la mezcla actual de Ant Design y MUI sin acoplar a las vistas consumidoras a una libreria concreta.

Adicionalmente, la propuesta generada automaticamente desde Jira contiene texto generico heredado del flujo `opsxj:new`, por lo que el diseno debe fijar con claridad el alcance tecnico real del cambio para que `specs` y `tasks` se construyan sobre una intencion consistente.

## Goals / Non-Goals

**Goals:**
- Exponer un componente `AppToolbar` reusable desde la capa UI compartida del proyecto.
- Definir una API tipada para componer titulo, descripcion, breadcrumbs, acciones primarias, acciones secundarias y contenido auxiliar.
- Resolver un comportamiento responsive consistente para desktop y mobile sin duplicar logica en cada modulo.
- Permitir que modulos como `gestionCorrespondencia` adopten la toolbar sin depender directamente de componentes base del proveedor UI.
- Mantener accesibilidad, orden semantico y soporte para acciones con iconografia.

**Non-Goals:**
- No rediseñar el layout completo del dashboard ni sustituir los componentes UI existentes.
- No introducir una dependencia nueva si el comportamiento puede resolverse con React, CSS Modules y las librerias ya presentes.
- No implementar logica de negocio de ningun modulo dentro de la toolbar.
- No definir aun variantes visuales para todos los modulos de producto; el primer alcance sera un contrato reusable con estilo base enterprise.

## Decisions

### 1. Ubicar `AppToolbar` en la capa `src/app/Components/UI`

`AppToolbar` se implementara en la misma capa que los demas wrappers compartidos para mantener una entrada unica de primitives reutilizables del proyecto. Esto evita que cada modulo cree su propia toolbar y conserva la trazabilidad OpenSpec sobre un capability de UI transversal.

Alternativas consideradas:
- Crear la toolbar dentro de `src/shared`: descartado porque el repo ya reserva `src/app/Components/UI` para primitives visuales de alto reuso.
- Crear toolbars por modulo: descartado porque duplicaria estilos, contratos y reglas responsive.

### 2. Definir una API por slots en lugar de props rigidas por caso de uso

La toolbar expondra regiones tipadas para:
- encabezado (`title`, `subtitle` o `description`)
- navegacion contextual (`breadcrumbs` opcional)
- acciones principales y secundarias (`actions`, `secondaryActions`)
- contenido auxiliar (`children` o `extra`)

Este enfoque permite cubrir modulos simples y escenarios mas ricos sin multiplicar variantes cerradas. La implementacion puede aceptar `ReactNode` en regiones controladas y props semanticas para el contenido basico, manteniendo tipado estricto.

Alternativas consideradas:
- Toolbar completamente declarativa via lista de items: descartado porque reduce flexibilidad para encabezados ricos y contenido contextual.
- Toolbar libre basada solo en `children`: descartado porque hace perder consistencia, accesibilidad y capacidad de documentar contrato en specs.

### 3. Componer la estructura visual con CSS Modules y layout nativo, no con un wrapper fuerte de Ant Design

La toolbar se apoyara en contenedores HTML semanticos, `CSS Modules` y utilidades React. Si se necesita un control puntual como `Dropdown`, `Tooltip` o iconos, se integrara de forma optativa, pero el contrato publico del componente no debe depender del shape exacto de Ant Design o MUI.

Esto protege a las vistas consumidoras de cambios de proveedor visual y alinea la estrategia ya usada en `AppButton` y `AppTabs`, donde el wrapper normaliza el contrato del proyecto.

Alternativas consideradas:
- Implementar la toolbar como wrapper directo de `PageHeader` o componente equivalente del vendor UI: descartado por acoplamiento y porque la libreria puede no cubrir el layout enterprise esperado.

### 4. Resolver responsive con colapso por zonas, no con variantes duplicadas

En desktop, la toolbar mostrara encabezado a la izquierda y acciones a la derecha. En pantallas estrechas, la estructura cambiara a stack vertical, priorizando:
1. contexto del encabezado
2. accion primaria
3. acciones secundarias y contenido auxiliar

Si el numero de acciones supera el espacio disponible, la implementacion debe permitir un patron de overflow controlado, preferiblemente mediante un menu contextual opcional en vez de wrap desordenado. La decision concreta del mecanismo quedara en implementacion, pero el diseno fija que la API debe prever acciones visibles y acciones colapsables.

Alternativas consideradas:
- Solo permitir wrap CSS automatico: descartado porque degrada alineacion y jerarquia visual en toolbars densas.
- Crear una version mobile separada: descartado por costo de mantenimiento y riesgo de divergencia.

### 5. Mantener accesibilidad y semantica como parte del contrato base

La toolbar debe renderizar una jerarquia semantica clara:
- contenedor con rol de region o `header` contextual cuando aplique
- titulo asociado visualmente al contexto principal
- orden de tabulacion estable
- acciones icon-only obligadas a definir nombre accesible

Las breadcrumbs y menus de overflow deben reutilizar patrones accesibles ya soportados por la libreria base o por primitives del proyecto.

Alternativas consideradas:
- Tratar la toolbar como contenedor puramente visual: descartado porque debilita navegacion por teclado y comprension del contexto por tecnologias asistivas.

## Risks / Trade-offs

- [Propuesta Jira ambigua] -> Mitigacion: fijar en specs el capability `app-toolbar` y usar este diseno como fuente de verdad para alcance funcional.
- [API demasiado flexible y dificil de mantener] -> Mitigacion: limitar slots publicos a regiones semanticas concretas y evitar props duplicadas para el mismo fin.
- [Responsive inconsistente entre modulos] -> Mitigacion: encapsular puntos de quiebre y reglas de overflow dentro del componente, no en consumidores.
- [Acoplamiento accidental a Ant Design o MUI] -> Mitigacion: exponer tipos y props del proyecto, usando vendor UI solo como detalle interno cuando sea necesario.
- [Toolbar sobrecargada con acciones de negocio] -> Mitigacion: documentar que la toolbar solo orquesta presentacion y composicion; la logica queda en hooks/paginas consumidoras.

## Migration Plan

1. Crear el spec del capability `app-toolbar` con requisitos de composicion, responsive y accesibilidad.
2. Implementar `AppToolbar` en `src/app/Components/UI/AppToolbar/` con export centralizado.
3. Agregar pruebas de comportamiento con Vitest + Testing Library para encabezado, acciones y comportamiento responsive/overflow segun el contrato que se defina.
4. Integrar un caso consumidor real, preferiblemente en `gestionCorrespondencia`, para validar adopcion y ergonomia.
5. Si la adopcion revela gaps en el contrato, ajustar spec y tareas antes de extender a otros modulos.

Rollback:
- Si la integracion genera regresiones visuales, retirar temporalmente el uso del componente en consumidores y conservar el primitive aislado hasta corregir el contrato.

## Open Questions

- La primera version no incluira motor de busqueda o filtros propios; solo expondra regiones reutilizables para alojarlos si un consumidor lo necesita.
- El overflow de acciones secundarias se resolvera con un menu contextual controlado por la propia toolbar, manteniendo visible la accion prioritaria.
- La variante sticky queda fuera del alcance inicial y se tratara como una extension futura si aparece un caso real de adopcion.
