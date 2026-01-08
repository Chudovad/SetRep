// Drag and Drop для упражнений в тренировке
// Перетаскивание работает только при захвате за иконку меню

window.workoutDragDrop = {
    // Текущий перетаскиваемый элемент
    draggedElement: null,
    draggedIndex: -1,
    placeholder: null,
    // Callback для вызова из Blazor
    onDropCallback: null,
    // Флаг для отслеживания перетаскивания
    isDragging: false,
    startX: 0,
    startY: 0,
    
    // Инициализация drag and drop для контейнера
    init: function(containerId, onDropCallback) {
        this.onDropCallback = onDropCallback;
        const container = document.getElementById(containerId);
        if (!container) return;
        
        const self = this;
        
        // Находим все элементы с классом drag-handle-icon
        const dragHandles = container.querySelectorAll('.drag-handle-icon');
        
        dragHandles.forEach((handle, index) => {
            const exerciseCard = handle.closest('.exercise-card-wrapper');
            if (!exerciseCard) return;
            
            // Устанавливаем data-index для идентификации элемента
            exerciseCard.setAttribute('data-exercise-index', index);
            
            // Добавляем обработчики только на иконку меню
            handle.addEventListener('mousedown', function(e) {
                e.stopPropagation(); // Предотвращаем всплытие события
                self.handleDragStart(e, exerciseCard);
            });
            handle.addEventListener('touchstart', function(e) {
                e.stopPropagation(); // Предотвращаем всплытие события
                self.handleDragStart(e, exerciseCard);
            }, { passive: false });
            
            // Предотвращаем клик на иконке меню
            handle.addEventListener('click', function(e) {
                e.preventDefault();
                e.stopPropagation();
            });
            
            // Предотвращаем стандартное поведение для иконки
            handle.style.userSelect = 'none';
            handle.style.webkitUserSelect = 'none';
        });
    },
    
    // Обработка начала перетаскивания
    handleDragStart: function(e, element) {
        // Предотвращаем скролл при начале перетаскивания
        e.preventDefault();
        e.stopPropagation();
        
        const self = window.workoutDragDrop;
        self.draggedElement = element;
        self.draggedIndex = parseInt(element.getAttribute('data-exercise-index')) || 0;
        self.isDragging = false;
        
        // Сохраняем начальные координаты
        self.startX = e.touches ? e.touches[0].clientX : e.clientX;
        self.startY = e.touches ? e.touches[0].clientY : e.clientY;
        
        // Предотвращаем клик на карточке во время перетаскивания
        element.style.pointerEvents = 'none';
        
        // Добавляем обработчики для всего документа
        document.addEventListener('mousemove', self.handleDragMove);
        document.addEventListener('mouseup', self.handleDragEnd);
        document.addEventListener('touchmove', self.handleDragMove, { passive: false });
        document.addEventListener('touchend', self.handleDragEnd);
        
        // Предотвращаем скролл страницы во время перетаскивания
        document.body.style.overflow = 'hidden';
    },
    
    // Обработка движения во время перетаскивания
    handleDragMove: function(e) {
        const self = window.workoutDragDrop;
        if (!self.draggedElement) return;
        
        const clientY = e.touches ? e.touches[0].clientY : e.clientY;
        const clientX = e.touches ? e.touches[0].clientX : e.clientX;
        
        // Проверяем, началось ли перетаскивание (движение больше 5px)
        const deltaX = Math.abs(clientX - self.startX);
        const deltaY = Math.abs(clientY - self.startY);
        
        if (!self.isDragging && (deltaX > 5 || deltaY > 5)) {
            self.isDragging = true;
            const draggedElement = self.draggedElement;
            
            // Добавляем класс для визуальной обратной связи
            draggedElement.classList.add('dragging');
            
            // Создаем placeholder
            self.createPlaceholder(draggedElement);
        }
        
        if (!self.isDragging) return;
        
        e.preventDefault();
        e.stopPropagation();
        
        const draggedElement = self.draggedElement;
        const container = draggedElement.closest('.exercise-list-container');
        
        if (!container) return;
        
        // Находим элемент под курсором
        const elements = Array.from(container.querySelectorAll('.exercise-card-wrapper:not(.dragging):not(.placeholder)'));
        let insertBeforeElement = null;
        let insertAtEnd = false;
        
        if (elements.length > 0) {
            const firstElement = elements[0];
            const lastElement = elements[elements.length - 1];
            const firstRect = firstElement.getBoundingClientRect();
            const lastRect = lastElement.getBoundingClientRect();
            
            // Проверяем, находится ли курсор выше первого элемента (вставка в начало)
            if (clientY < firstRect.top) {
                insertBeforeElement = firstElement;
            }
            // Проверяем, находится ли курсор ниже последнего элемента (вставка в конец)
            // Используем небольшую зону нечувствительности для стабильности
            else if (clientY >= lastRect.bottom - 10) {
                insertAtEnd = true;
            }
            // Проверяем все элементы, чтобы найти правильную позицию
            else {
                for (let i = 0; i < elements.length; i++) {
                    const rect = elements[i].getBoundingClientRect();
                    const elementTop = rect.top;
                    const elementBottom = rect.bottom;
                    const elementCenterY = elementTop + rect.height / 2;
                    
                    // Если курсор выше центра элемента, вставляем перед ним
                    if (clientY >= elementTop && clientY < elementCenterY) {
                        insertBeforeElement = elements[i];
                        break;
                    }
                    // Если курсор ниже центра элемента
                    else if (clientY >= elementCenterY && clientY < elementBottom) {
                        // Вставляем после этого элемента (перед следующим)
                        if (i < elements.length - 1) {
                            insertBeforeElement = elements[i + 1];
                        } else {
                            // Это последний элемент, вставляем после него
                            insertAtEnd = true;
                        }
                        break;
                    }
                }
            }
        }
        
        // Перемещаем placeholder
        if (insertAtEnd) {
            // Вставляем в конец, если placeholder еще не там
            // Проверяем, является ли placeholder последним элементом упражнения (игнорируя draggedElement)
            const children = Array.from(container.children);
            const exerciseChildren = children.filter(child => 
                child.classList.contains('exercise-card-wrapper') && 
                child !== draggedElement
            );
            const isPlaceholderLast = exerciseChildren.length > 0 && 
                                     exerciseChildren[exerciseChildren.length - 1] === self.placeholder;
            
            if (!isPlaceholderLast) {
                container.appendChild(self.placeholder);
            }
        } else if (insertBeforeElement && insertBeforeElement !== self.placeholder) {
            // Проверяем, не находится ли placeholder уже перед этим элементом
            // Если следующий sibling placeholder не является целевым элементом, перемещаем
            if (self.placeholder.nextSibling !== insertBeforeElement) {
                container.insertBefore(self.placeholder, insertBeforeElement);
            }
        }
        
        // Обновляем позицию перетаскиваемого элемента
        draggedElement.style.position = 'fixed';
        draggedElement.style.left = (clientX - 20) + 'px';
        draggedElement.style.top = (clientY - 20) + 'px';
        draggedElement.style.width = draggedElement.offsetWidth + 'px';
        draggedElement.style.opacity = '0.5';
        draggedElement.style.pointerEvents = 'none';
        draggedElement.style.zIndex = '1000';
    },
    
    // Обработка окончания перетаскивания
    handleDragEnd: function(e) {
        const self = window.workoutDragDrop;
        if (!self.draggedElement) return;
        
        e.preventDefault();
        e.stopPropagation();
        
        const draggedElement = self.draggedElement;
        const placeholder = self.placeholder;
        const container = draggedElement.closest('.exercise-list-container');
        const wasDragging = self.isDragging;
        
        // Определяем новый индекс на основе позиции placeholder
        let newIndex = -1;
        if (wasDragging && placeholder && placeholder.parentNode) {
            // Получаем все элементы упражнений в DOM порядке (исключая placeholder и draggedElement)
            const allExerciseElements = Array.from(container.querySelectorAll('.exercise-card-wrapper:not(.placeholder):not(.dragging)'));
            
            // Находим позицию placeholder среди всех дочерних элементов контейнера
            const placeholderIndex = Array.from(container.children).indexOf(placeholder);
            
            // Проверяем, находится ли placeholder в конце
            const isAtEnd = placeholderIndex === container.children.length - 1 || 
                           (placeholderIndex === container.children.length - 2 && 
                            container.children[container.children.length - 1] === draggedElement);
            
            if (isAtEnd) {
                // Если placeholder в конце, индекс равен количеству элементов упражнений (последний индекс + 1)
                // Но так как мы перемещаем существующий элемент, нужно вычесть 1, если он не был последним
                // Проще всего - просто использовать количество элементов минус 1 (последний валидный индекс)
                newIndex = allExerciseElements.length > 0 ? allExerciseElements.length : 0;
            } else {
                // Подсчитываем элементы упражнений перед placeholder
                newIndex = 0;
                for (let i = 0; i < placeholderIndex; i++) {
                    const child = container.children[i];
                    // Считаем только элементы упражнений (не placeholder, не draggedElement)
                    if (child.classList.contains('exercise-card-wrapper') &&
                        child !== placeholder &&
                        child !== draggedElement) {
                        newIndex++;
                    }
                }
            }
        }
        
        // Восстанавливаем стили
        draggedElement.classList.remove('dragging');
        draggedElement.style.position = '';
        draggedElement.style.left = '';
        draggedElement.style.top = '';
        draggedElement.style.width = '';
        draggedElement.style.opacity = '';
        draggedElement.style.pointerEvents = '';
        draggedElement.style.zIndex = '';
        
        // Удаляем placeholder
        if (placeholder && placeholder.parentNode) {
            placeholder.parentNode.removeChild(placeholder);
        }
        
        // Вызываем callback из Blazor, если индекс изменился
        if (wasDragging && newIndex >= 0 && newIndex !== self.draggedIndex && self.onDropCallback) {
            self.onDropCallback.invokeMethodAsync('OnExerciseDropped', self.draggedIndex, newIndex);
        }
        
        // Если было перетаскивание, предотвращаем клик
        if (wasDragging) {
            // Временно блокируем клик на элементе
            setTimeout(() => {
                if (draggedElement) {
                    draggedElement.style.pointerEvents = '';
                }
            }, 100);
        } else {
            // Если не было перетаскивания, сразу восстанавливаем
            draggedElement.style.pointerEvents = '';
        }
        
        // Очищаем состояние
        self.draggedElement = null;
        self.draggedIndex = -1;
        self.placeholder = null;
        self.isDragging = false;
        self.startX = 0;
        self.startY = 0;
        
        // Удаляем обработчики
        document.removeEventListener('mousemove', self.handleDragMove);
        document.removeEventListener('mouseup', self.handleDragEnd);
        document.removeEventListener('touchmove', self.handleDragMove);
        document.removeEventListener('touchend', self.handleDragEnd);
        
        // Восстанавливаем скролл
        document.body.style.overflow = '';
    },
    
    // Создание placeholder элемента
    createPlaceholder: function(element) {
        const self = window.workoutDragDrop;
        const placeholder = document.createElement('div');
        placeholder.className = 'exercise-card-wrapper placeholder';
        placeholder.style.height = element.offsetHeight + 'px';
        placeholder.style.marginBottom = '12px';
        placeholder.style.border = '2px dashed #ccc';
        placeholder.style.borderRadius = '12px';
        placeholder.style.backgroundColor = '#f5f5f5';
        
        self.placeholder = placeholder;
        element.parentNode.insertBefore(placeholder, element);
    },
    
    // Обновление индексов после изменения порядка (вызывается из Blazor)
    updateIndices: function(containerId) {
        const container = document.getElementById(containerId);
        if (!container) return;
        
        const elements = container.querySelectorAll('.exercise-card-wrapper:not(.placeholder)');
        elements.forEach((element, index) => {
            element.setAttribute('data-exercise-index', index);
        });
    }
};
