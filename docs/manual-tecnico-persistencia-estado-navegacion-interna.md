# Manual Tecnico: Persistencia de Estado y Navegacion Interna en Modulos SPA React

## 1. Objetivo

Este manual define una metodologia tecnica reutilizable para resolver navegacion interna entre componentes o vistas de un modulo SPA React sin perder el estado critico del flujo.

El documento formaliza dos enfoques:

- Metodo A: contenedor persistente con vistas internas sin cambio de ruta.
- Metodo B: layout persistente con subrutas internas y `Outlet`.

La meta no es eliminar todos los re-render del arbol React. La meta arquitectonica correcta es evitar el desmontaje del contenedor que posee el estado critico.

## 2. Problema Que Resuelve

En modulos con formularios largos, flujos multi-step, tabs funcionales o subpantallas operativas, es frecuente necesitar:

- cambiar entre componentes `tsx`
- regresar a una vista anterior sin perder formulario, seleccion o contexto
- evitar reinicializaciones innecesarias
- mantener consistencia con React Router y la arquitectura SPA existente

El problema real suele aparecer cuando el ownership del estado queda en una vista hija que se desmonta al navegar.

## 3. Contexto Arquitectonico

Esta metodologia asume una SPA con React Router y layouts persistentes. En este repositorio ya existe ese patron:

```txt
/dashboard
  └─ DashboardLayout
      └─ Outlet
          ├─ Home
          ├─ Workflow
          └─ Radicacion
```

Ese modelo demuestra que:

- un layout padre puede permanecer montado
- las vistas hijas pueden cambiar dentro de `Outlet`
- el estado puede preservarse si vive en el contenedor correcto

La metodologia aplica especialmente cuando el sistema usa:

- React 19
- React Router v7
- layouts persistentes
- React Query para cache de datos remotos
- modulos por dominio

## 4. Principio Arquitectonico Base

El principio rector es:

`El estado critico debe vivir en el componente que no se desmonta durante la navegacion del flujo.`

Consecuencias practicas:

- El ownership del estado debe ser explicito.
- El formulario no debe ser dueño del flujo completo si la vista puede desmontarse.
- La carga principal de datos no debe duplicarse en varias capas sin necesidad.
- React Query sirve para cache remota, pero no sustituye por si sola el ownership del estado del flujo.

## 5. Metodo A: Contenedor Persistente con Vistas Internas

### 5.1 Objetivo

Resolver navegacion entre vistas relacionadas dentro de una sola ruta, manteniendo un contenedor persistente local del modulo.

### 5.2 Idea Central

El modulo no cambia de ruta. Lo que cambia es la vista interna activa.

El contenedor principal se monta una sola vez y administra:

- formulario
- datos cargados
- seleccion actual
- vista activa
- acciones compartidas

### 5.3 Arquitectura

```txt
/modulo
  └─ ModuloPage
      └─ ModuloWorkspace
          ├─ estado compartido
          ├─ toolbar / tabs
          ├─ VistaPrincipal
          ├─ VistaSecundaria
          └─ VistaTerciaria
```

### 5.4 Flujo Paso a Paso

1. Identificar el estado critico del flujo.
2. Crear un contenedor persistente del modulo, por ejemplo `ModuloWorkspace`.
3. Mover al contenedor:
   - `Form.useForm()`
   - carga principal de datos
   - estado de seleccion o borrador
4. Definir un estado de navegacion interna, por ejemplo `activeView`.
5. Renderizar las vistas como paneles internos controlados por estado.
6. Pasar `form`, datos y acciones a las vistas hijas por props o contexto.
7. Verificar que cambiar de vista no reinicialice el estado critico.

### 5.5 Ejemplo Practico

```txt
/dashboard/radicacion
  └─ RadicacionPage
      └─ RadicacionWorkspace
          ├─ Tabs
          ├─ RadicacionForm
          ├─ CapDocument
          └─ GestionRadicados
```

Ejemplo conceptual:

```tsx
function RadicacionWorkspace() {
  const [activeView, setActiveView] = useState<"form" | "captura" | "gestion">("form");
  const [form] = Form.useForm();
  const plantillaQuery = useCamposPlantilla();

  return (
    <>
      <RadicacionTabs activeView={activeView} onChange={setActiveView} />
      {activeView === "form" && <RadicacionForm form={form} />}
      {activeView === "captura" && <CapDocument form={form} />}
      {activeView === "gestion" && <GestionRadicados form={form} />}
    </>
  );
}
```

### 5.6 Casos de Uso Recomendados

- Flujos cerrados dentro de una sola tarea de negocio.
- Formularios que deben convivir con tabs funcionales.
- Alternancia frecuente entre paneles relacionados.
- Casos donde no se necesita URL distinta por subvista.

### 5.7 Ventajas

- Menor complejidad de routing.
- Preservacion de estado local muy alta.
- Menor costo de implementacion.
- Muy adecuado para formularios largos.

### 5.8 Limites

- No ofrece URL independiente por subvista.
- El historial del navegador no representa cada panel interno.
- Si luego se necesita deep-linking, puede requerir evolucion a Metodo B.

## 6. Metodo B: Layout Persistente con Subrutas

### 6.1 Objetivo

Permitir navegacion interna con URL propia por subvista, sin perder el estado compartido del modulo.

### 6.2 Idea Central

El modulo define un layout propio que permanece montado mientras cambian las subrutas hijas.

### 6.3 Arquitectura

```txt
/modulo/*
  └─ ModuloLayout
      ├─ estado compartido
      ├─ navegacion interna
      └─ Outlet
          ├─ index
          ├─ captura
          ├─ gestion
          └─ detalle
```

### 6.4 Flujo Paso a Paso

1. Identificar que el modulo necesita URL por subvista.
2. Crear un `ModuloLayout` persistente.
3. Definir subrutas hijas bajo el layout.
4. Mover al layout:
   - formulario o estado del flujo
   - carga principal de datos
   - acciones compartidas
5. Exponer el estado por `Outlet context`, contexto o store justificado.
6. Convertir cada subvista en pagina hija consumidora del layout.
7. Validar que cambiar de subruta no destruya el contenedor del estado.

### 6.5 Ejemplo Practico

```txt
/dashboard/radicacion/*
  └─ RadicacionLayout
      ├─ Tabs o header
      ├─ estado compartido
      └─ Outlet
          ├─ index -> RadicacionFormPage
          ├─ captura -> CapDocumentPage
          └─ gestion -> GestionRadicadosPage
```

Ejemplo conceptual:

```tsx
function RadicacionLayout() {
  const [form] = Form.useForm();
  const plantillaQuery = useCamposPlantilla();

  return (
    <Outlet
      context={{
        form,
        plantilla: plantillaQuery.data,
      }}
    />
  );
}
```

### 6.6 Casos de Uso Recomendados

- Necesidad de URL propia por subpantalla.
- Deep-linking.
- Historial navegable con back/forward.
- Subdominios que creceran con varias vistas internas.

### 6.7 Ventajas

- Mejor trazabilidad de navegacion.
- URLs compartibles.
- Escalabilidad superior del modulo.
- Compatible con layouts padres ya existentes.

### 6.8 Limites

- Mayor complejidad de routing.
- Requiere ownership del estado mas disciplinado.
- Si el estado sigue en una vista hija, se volvera a perder al cambiar de subruta.

## 7. Diferencias Entre Metodo A y Metodo B

| Criterio | Metodo A | Metodo B |
|---|---|---|
| URL por subvista | No | Si |
| Complejidad inicial | Baja | Media |
| Preservacion de estado local | Muy alta | Alta si el estado vive en el layout |
| Historial navegador | Limitado | Completo |
| Deep-linking | No | Si |
| Escalabilidad como subdominio | Media | Alta |
| Adecuado para flujo cerrado | Si | Si |
| Adecuado para navegacion formal interna | No ideal | Si |

## 8. Criterios de Decision

### Usar Metodo A cuando:

- El flujo es una sola unidad de negocio.
- No se necesita URL por subvista.
- El usuario alterna muchas veces entre paneles relacionados.
- La prioridad es no perder formulario, borrador o seleccion.

### Usar Metodo B cuando:

- Cada subvista necesita URL propia.
- Se requiere historial y navegacion del navegador.
- Se necesitan enlaces compartibles.
- El modulo crecera como subdominio con varias paginas internas.

### Regla de decision rapida

Si el problema es "volver sin perder el estado del flujo" y no se necesita URL distinta, se prefiere Metodo A.

Si el problema es "tener navegacion interna real del modulo con URL y sin perder contexto", se prefiere Metodo B.

## 9. Buenas Practicas

- Centralizar la carga principal de datos en el contenedor o layout persistente.
- Mantener un solo dueño del estado critico.
- Diferenciar entre datos remotos, estado del flujo y estado visual efimero.
- Mantener las vistas hijas como consumidoras del estado, no como dueñas del flujo.
- Usar nombres explicitos para los contenedores persistentes:
  - `Workspace`
  - `Layout`
  - `Shell`
- Reutilizar `Outlet context` o contexto solo cuando el flujo realmente comparte estado.
- Elegir store global solo si el estado debe sobrevivir incluso al salir completamente del modulo.

## 10. Riesgos y Trade-offs

- Duplicar la carga de datos en layout y vistas hijas.
  - Mitigacion: definir un owner unico de la consulta.
- Dejar `Form.useForm()` dentro de una vista desmontable.
  - Mitigacion: subir la instancia al contenedor persistente.
- Sobredimensionar el modulo con subrutas cuando bastaban vistas internas.
  - Mitigacion: usar Metodo B solo si negocio necesita URL o escalabilidad real.
- Intentar resolver el problema con hacks de keep-alive como primera opcion.
  - Mitigacion: corregir primero ownership del estado y arquitectura del flujo.
- Confundir re-render con remount.
  - Mitigacion: medir que componente pierde estado y donde vive la fuente de verdad.

## 11. Aplicacion en Otros Modulos del Sistema

La metodologia puede aplicarse en:

- `radicacion`
- `workflow`
- gestion documental
- bandejas operativas
- administracion
- configuracion avanzada
- asistentes multi-step

Preguntas guia para reutilizar el enfoque:

1. Que estado no puede perderse.
2. Quien debe ser dueño de ese estado.
3. La navegacion es visual o funcional.
4. Se necesita URL por subvista.
5. El usuario vuelve frecuentemente atras.
6. El modulo crecera como subdominio.

### Patron reutilizable A

`<ModuloWorkspace />`

Adecuado para modulos que permanecen en una sola ruta y cambian paneles internos.

### Patron reutilizable B

`<ModuloLayout /> + subrutas`

Adecuado para modulos que necesitan navegacion interna formal y ownership compartido del flujo.

## 12. Antipatrones a Evitar

- Cargar el mismo dato principal desde varias capas sin necesidad.
- Dejar toda la logica del flujo dentro del formulario.
- Crear subrutas sin subir el estado compartido.
- Meter todo el estado en un store global sin criterio.
- Diseñar para "cero rerender" en vez de diseñar ownership correcto.
- Ocultar un problema de arquitectura con soluciones de cache de vistas como primer recurso.

## 13. Criterios de Aceptacion Tecnicos Sugeridos

- El contenedor principal del flujo no se desmonta al cambiar entre las vistas contempladas por la solucion elegida.
- El estado critico se conserva al regresar.
- No se duplican cargas principales sin justificacion tecnica.
- Las vistas hijas consumen estado compartido del contenedor o layout de forma explicita.
- La solucion respeta React Router y la arquitectura SPA vigente.
- El codigo resultante es reutilizable y mantenible para otros modulos.

## 14. Prompt Profesional para Jira / Codex / IA

```text
Actua como ingeniero de software senior especializado en React, React Router, arquitectura SPA y refactorizacion incremental.

Objetivo:
Implementar una metodologia de persistencia de estado y navegacion interna en el modulo [NOMBRE_DEL_MODULO], respetando la arquitectura existente del sistema, la reutilizacion de componentes, la calidad tecnica y los criterios de aceptacion.

Contexto tecnico:
- El sistema es una SPA en React.
- La arquitectura actual usa layouts persistentes con React Router y `Outlet`.
- Existen componentes TSX que necesitan navegar entre si sin perder el estado del componente principal.
- El problema a resolver no es eliminar todo re-render, sino evitar el desmontaje del contenedor que posee el estado critico.
- La solucion debe seguir uno de estos enfoques:
  1. Metodo A: contenedor persistente con vistas internas sin cambio de ruta.
  2. Metodo B: layout persistente con subrutas internas y `Outlet`.

Instrucciones:
1. Analiza la estructura actual del modulo y detecta:
   - componente principal
   - dueño actual del estado critico
   - hooks de carga de datos
   - puntos donde hoy se pierde estado o se duplican consultas
2. Define cual metodo aplica mejor, A o B, segun:
   - necesidad o no de URL propia por subvista
   - complejidad funcional
   - frecuencia de regreso entre vistas
   - escalabilidad futura
3. Implementa la solucion seleccionada respetando estos principios:
   - un solo dueño del estado critico
   - centralizacion de carga de datos
   - separacion entre estado compartido y vistas hijas
   - reutilizacion de componentes existentes
   - refactor incremental, sin romper comportamiento actual
4. Si eliges Metodo A:
   - crea un contenedor persistente tipo `Workspace`
   - mueve alli el estado compartido, la carga principal y la instancia del formulario si aplica
   - convierte las vistas internas en paneles controlados por estado o tabs
5. Si eliges Metodo B:
   - crea un layout persistente del modulo
   - define subrutas internas bajo ese layout
   - expone el estado compartido mediante `Outlet context`, contexto o store justificado
6. Evita:
   - duplicacion de consultas
   - acoplamiento innecesario
   - soluciones hacky de keep-alive como primera opcion
   - refactors masivos no necesarios
7. Conserva estilo, patrones y convenciones del repositorio.

Entregables esperados:
- implementacion funcional
- explicacion breve de la solucion elegida
- justificacion arquitectonica de por que se eligio Metodo A o B
- detalle de archivos modificados
- pruebas o validacion de navegacion y persistencia de estado

Criterios de aceptacion:
- el contenedor principal no se desmonta al cambiar entre vistas del flujo definido
- el estado critico se conserva al regresar
- no se duplican cargas principales sin razon tecnica
- la solucion respeta React Router y la arquitectura actual
- el codigo es mantenible, escalable y reutilizable en otros modulos

Si detectas ambiguedad funcional, resuelvela con la alternativa mas consistente con la arquitectura existente y documenta la decision.
```

## 15. Uso Recomendado del Manual

Este documento debe usarse como referencia previa antes de:

- crear un refactor de navegacion interna
- mover ownership de formularios o flujos
- convertir tabs funcionales en subrutas
- pedir a Codex o a otra IA que implemente preservacion de estado en un modulo

Si el cambio implica implementacion real, se recomienda complementar este manual con:

- ticket Jira claro
- cambio OpenSpec con `proposal`, `design`, `specs` y `tasks`
- validacion del ownership del estado antes de codificar
