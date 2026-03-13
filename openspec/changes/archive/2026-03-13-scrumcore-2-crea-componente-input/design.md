## Context

El ticket `SCRUMCORE-2` requiere crear un input estandar reusable para la SPA, desacoplando a las vistas consumidoras del control base del proveedor UI y preservando consistencia visual, semantica y de accesibilidad. El repositorio ya dispone de una capa compartida en `src/app/Components/UI`, usa CSS Modules en varios modulos y no tiene hoy un componente base equivalente para entradas de texto reutilizables.

El cambio es transversal porque introduce una nueva pieza del design system que puede ser adoptada por formularios, filtros y flujos de captura en distintos modulos. Tambien necesita fijar un contrato publico claro para valor, eventos, ayuda contextual, estados visuales y accesibilidad antes de implementar adopciones posteriores.

## Goals / Non-Goals

**Goals:**
- Crear `AppInput` como abstraccion tipada sobre el control de entrada base de la libreria UI en `src/app/Components/UI/AppInput/`.
- Exponer una API publica propia con `value`, `defaultValue`, `onChange`, `placeholder`, `label`, `helperText`, `disabled`, `error` y `className`.
- Mantener compatibilidad con props utiles heredadas del proveedor UI sin exponer directamente su semantica visual a las vistas.
- Implementar estilos con CSS Modules para alinear los campos de texto con una apariencia enterprise coherente con la UI actual.
- Garantizar accesibilidad en foco visible, asociacion correcta label-control, ayuda contextual y semantica de error.
- Agregar pruebas con Vitest + Testing Library y documentacion local del componente.

**Non-Goals:**
- Migrar en este ticket todos los inputs existentes del proyecto.
- Introducir una libreria nueva de formularios o un design system externo.
- Acoplar `AppInput` a modulos de dominio o reglas especificas de formularios concretos.
- Redefinir el theme global del proveedor UI o la arquitectura SPA.

## Decisions

### Decision 1: Ubicar AppInput en `src/app/Components/UI/AppInput/`
- **Decision:** crear una carpeta dedicada con `AppInput.tsx`, `AppInput.module.css`, `AppInput.test.tsx`, `index.ts` y `README.md`.
- **Rationale:** `src/app/Components/UI` ya funciona como capa compartida y es el punto correcto para componentes base no ligados a dominio. La carpeta dedicada mantiene cohesion y simplifica evolucion, pruebas y documentacion.
- **Alternatives considered:** ubicarlo en `src/shared` o incrustarlo dentro de un modulo de formularios; se descartan por mezclar responsabilidades o debilitar el contrato compartido.

### Decision 2: Componer sobre el control de entrada del proveedor UI en lugar de reimplementar un input nativo
- **Decision:** construir `AppInput` como wrapper sobre el control base ya disponible en la libreria UI del proyecto, usando un contrato tipado con herencia controlada (`Omit<ComponentProps<...>, ...>`) para redefinir la API propia.
- **Rationale:** reutiliza comportamiento probado del proveedor UI y evita que cada vista dependa directamente de su API. Tambien reduce divergencias en accesibilidad y estados basicos.
- **Alternatives considered:** renderizar un `<input>` nativo o exponer directamente el componente del proveedor; se descartan por perder consistencia o mantener el acoplamiento actual.

### Decision 3: Separar el lenguaje visual del contrato del proveedor UI
- **Decision:** mapear estados como `error`, `disabled` y la variante base del input a clases CSS Modules propias, apoyandose solo de forma limitada en props internas del proveedor cuando aporten comportamiento.
- **Rationale:** el design system debe controlar la presentacion compartida del proyecto y no depender totalmente de nombres o variantes externas.
- **Alternatives considered:** usar solo estilos o props del proveedor UI; se descarta porque limita la evolucion visual y deja a las vistas expuestas a detalles del framework UI.

### Decision 4: Soportar composicion completa para formularios sin logica de negocio
- **Decision:** incluir `label`, `helperText`, `placeholder`, `value`, `defaultValue` y `onChange` en el contrato publico, pero sin integrar validaciones de dominio ni dependencias a formularios especificos.
- **Rationale:** el componente debe ser util en formularios controlados y no controlados, manteniendo bajo acoplamiento y reutilizacion amplia.
- **Alternatives considered:** limitarlo a un simple wrapper visual sin label ni helper text; se descarta por obligar a repetir estructura accesible en cada vista.

### Decision 5: Validar accesibilidad y comportamiento con pruebas de contrato
- **Decision:** agregar pruebas de render, sincronizacion de valor, `onChange`, estados `disabled/error`, label, helper text, placeholder y composicion de clases.
- **Rationale:** `AppInput` sera una pieza base en formularios; una suite de comportamiento reduce regresiones y hace verificables los escenarios del spec.
- **Alternatives considered:** snapshots o pruebas solo visuales; se descartan por baja señal y menor cobertura de accesibilidad y contrato funcional.

## Risks / Trade-offs

- **[Risk]** El contrato propio de `AppInput` puede entrar en conflicto con props heredadas del proveedor UI.  
  **Mitigation:** limitar props redefinidas mediante `Omit` y documentar con claridad la API publica soportada.
- **[Risk]** Los estilos locales pueden competir con la hoja interna del proveedor UI.  
  **Mitigation:** encapsular estilos con CSS Modules y evitar overrides fragiles dependientes de clases internas no estables.
- **[Risk]** Incluir `label` y `helperText` dentro del componente puede volverlo mas opinionado.  
  **Mitigation:** mantener la estructura minima necesaria para accesibilidad y dejar fuera logica de layout compleja o reglas de dominio.
- **[Risk]** La adopcion inicial puede quedar limitada si no existe un barrel claro o ejemplos de uso.  
  **Mitigation:** exponer el componente desde la capa UI y acompañarlo con README y ejemplos listos para copiar.

## Migration Plan

1. Crear la carpeta `src/app/Components/UI/AppInput/` con implementacion, estilos, tests, barrel export y README.
2. Exponer el componente desde la capa UI compartida siguiendo la convencion del repo.
3. Ejecutar la suite de pruebas del componente y registrar evidencia en OpenSpec.
4. Dejar el componente listo para adopcion progresiva en futuros tickets de formularios.

Rollback:
- Revertir los archivos de `AppInput` y sus exports; no hay migracion de datos ni efectos persistentes fuera del frontend.

## Open Questions

- Confirmar si el proveedor UI elegido para `AppInput` debe ser Ant Design, MUI o una adaptacion neutral segun el patron ya usado por el equipo.
- Confirmar si el componente debe incluir soporte inicial para `type` diferentes a texto plano (`password`, `email`, `number`) en este ticket o en iteraciones posteriores.
- Verificar si el equipo quiere incluir desde el inicio soporte visual para prefijos/sufijos o dejarlo fuera del alcance minimo del ticket.
