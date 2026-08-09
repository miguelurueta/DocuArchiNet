class TooltipDaGestor {
    constructor(options = {}) {
        // Default configuration
        const defaultSettings = {
            iconSize: 24,  // Tamaño del icono
            tooltipBackground: '#333',  // Color de fondo del tooltip
            tooltipTextColor: '#fff',  // Color del texto del tooltip
            tooltipPosition: 'top',  // Posición del tooltip (no se usará, se posicionará dinámicamente)
            tooltipDelay: 300,  // Retraso para mostrar el tooltip
            iconColor: '#007bff',  // Color de fondo del icono
            hoverColor: '#0056b3',  // Color de fondo al hacer hover
            iconClass: 'bi-info-circle',  // Bootstrap icon class
            tooltipText: 'Este es el texto del tooltip',  // Texto por defecto
            targetElement: document.body,  // Elemento en el que se añadirá el tooltip
            offsetX: 10,  // Desplazamiento en X del tooltip
            offsetY: 20,  // Desplazamiento en Y del tooltip
            maxWidth: 250,  // Max ancho del tooltip
        };

        this.settings = { ...defaultSettings, ...options };
        this.tooltipElement = null;
        this.iconElement = null;
    }

    // Inicializador: Añadir el icono y el tooltip al contenedor
    init(targetElement = this.settings.targetElement) {
        if (!targetElement) {
            console.error('Elemento objetivo no encontrado');
            return;
        }

        // Crear el icono (ancla)
        this.iconElement = document.createElement('a');
        this.iconElement.classList.add('tooltipDaGestor-icon');
        this.iconElement.innerHTML = `<i class="bi ${this.settings.iconClass}"></i>`; // Icono de Bootstrap

        // Establecer el tamaño y color del icono
        this.iconElement.style.fontSize = `${this.settings.iconSize}px`;
        this.iconElement.style.color = this.settings.iconColor;
        this.iconElement.style.cursor = 'pointer';  // Cambiar el cursor para indicar interacción

        // Agregar el icono al contenedor objetivo
        targetElement.appendChild(this.iconElement);

        // Crear el tooltip
        this.tooltipElement = document.createElement('div');
        this.tooltipElement.classList.add('tooltipDaGestor');
        this.tooltipElement.innerHTML = this.settings.tooltipText;

        // Establecer el estilo del tooltip
        this.tooltipElement.style.backgroundColor = this.settings.tooltipBackground;
        this.tooltipElement.style.color = this.settings.tooltipTextColor;
        this.tooltipElement.style.position = 'absolute';  // Tooltip sigue al mouse
        this.tooltipElement.style.maxWidth = `${this.settings.maxWidth}px`;  // Limitar el tamaño máximo
        this.tooltipElement.style.display = 'none';  // Inicialmente oculto

        // Agregar el tooltip al contenedor objetivo
        targetElement.appendChild(this.tooltipElement);

        // Eventos del icono
        this.iconElement.addEventListener('mouseenter', () => this.showTooltip());
        this.iconElement.addEventListener('mouseleave', () => this.hideTooltip());
        this.iconElement.addEventListener('mousemove', (e) => this.positionTooltip(e));
    }

    // Posicionar el tooltip según la posición del ratón
    positionTooltip(event) {
        const tooltipWidth = this.tooltipElement.offsetWidth;
        const tooltipHeight = this.tooltipElement.offsetHeight;

        const mouseX = event.pageX;
        const mouseY = event.pageY;

        const offsetX = this.settings.offsetX;
        const offsetY = this.settings.offsetY;

        const windowWidth = window.innerWidth;
        const windowHeight = window.innerHeight;

        // Calcular la posición del tooltip para evitar que se salga de la pantalla
        let tooltipX = mouseX + offsetX;
        let tooltipY = mouseY + offsetY;

        // Ajustar la posición si el tooltip se va fuera de la ventana en el eje X
        if (tooltipX + tooltipWidth > windowWidth) {
            tooltipX = windowWidth - tooltipWidth - offsetX;
        }

        // Ajustar la posición si el tooltip se va fuera de la ventana en el eje Y
        if (tooltipY + tooltipHeight > windowHeight) {
            tooltipY = windowHeight - tooltipHeight - offsetY;
        }

        // Posicionar el tooltip
        this.tooltipElement.style.left = `${tooltipX}px`;
        this.tooltipElement.style.top = `${tooltipY}px`;
    }

    // Mostrar el tooltip
    showTooltip() {
        this.tooltipElement.style.display = 'block'; // Mostrar el tooltip
    }

    // Ocultar el tooltip
    hideTooltip() {
        this.tooltipElement.style.display = 'none'; // Ocultar el tooltip
    }

    // Actualizar la configuración del tooltip después de la inicialización
    updateSettings(newSettings) {
        this.settings = { ...this.settings, ...newSettings };

        // Actualizar el contenido del tooltip
        this.tooltipElement.innerHTML = this.settings.tooltipText;
        this.iconElement.style.fontSize = `${this.settings.iconSize}px`;
        this.tooltipElement.style.backgroundColor = this.settings.tooltipBackground;
        this.tooltipElement.style.color = this.settings.tooltipTextColor;
    }

    // Eliminar el tooltip y el icono del DOM
    destroy() {
        if (this.iconElement) {
            this.iconElement.removeEventListener('mouseenter', () => this.showTooltip());
            this.iconElement.removeEventListener('mouseleave', () => this.hideTooltip());
            this.iconElement.removeEventListener('mousemove', (e) => this.positionTooltip(e));

            this.iconElement.remove();
        }

        if (this.tooltipElement) {
            this.tooltipElement.remove();
        }
    }
}



// Ejemplo de uso del plugin con inicialización simple:
const tooltip = new TooltipDaGestor({
    iconSize: 30,
    tooltipText: 'Haz clic aquí para más información',
    tooltipBackground: '#222',
    tooltipTextColor: '#fff',
    iconClass: 'bi-info-circle-fill'
});

tooltip.init(document.querySelector('.tooltipDaGestor-container'));

// Actualizar la configuración del tooltip si es necesario
tooltip.updateSettings({
    tooltipText: 'Nuevo texto para el tooltip'
});

// Destruir el tooltip cuando ya no se necesite
// tooltip.destroy();
