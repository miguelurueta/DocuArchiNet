# Geometría estable de la interfaz

```mermaid
flowchart TB
    Trigger[Enviar a usuario<br/>ctw-action-slot--handoff-user] --> Dialog[Modal de usuario<br/>altura fija: 42 rem]
    Dialog --> Search[Campo de búsqueda]
    Search --> Loading[Estado cargando]
    Loading --> Results[Tabla o tarjetas de resultados]
    Results --> Scroll[Desplazamiento vertical interno reservado]

    Loading -. conserva tamaño y posición .-> Dialog
    Results -. conserva tamaño y posición .-> Dialog
```

El control nuevo recibe las mismas clases de ubicación que el control sustituido. El cuerpo del modal mantiene scroll vertical reservado, así la carga, una lista vacía o los resultados no cambian el tamaño del diálogo.
