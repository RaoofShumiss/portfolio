// Gestructureerde databron: de projectkaarten worden uit deze array opgebouwd.
const projects = [
    {
        id: 'hotel-simulatie',
        title: 'Hotel Simulatie Systeem',
        description: 'Een Java applicatie voor het simuleren van hotel operaties met gasten, liften en verschillende faciliteiten.',
        category: 'java',
        technologies: ['Java', 'Object-georiënteerd programmeren']
    },
    {
        id: 'portfolio-website',
        title: 'Portfolio Website',
        description: 'Een responsieve portfolio website gebouwd met HTML5 en CSS3.',
        category: 'web',
        technologies: ['HTML5', 'CSS3', 'Responsive Design']
    }
];

function filterProjects(projects, category) {
    return projects.filter((project) => category === 'all' || project.category === category);
}

function createProjectCard(project) {
    const article = document.createElement('article');
    article.dataset.projectId = project.id;
    article.dataset.category = project.category;
    const title = document.createElement('h3');
    title.textContent = project.title;
    const description = document.createElement('p');
    description.textContent = project.description;
    const technologies = document.createElement('p');
    const label = document.createElement('strong');
    label.textContent = 'Technologieën:';
    technologies.append(label, ' ' + project.technologies.join(', '));
    article.append(title, description, technologies);
    return article;
}

function renderProjects(projectList, visibleProjects) {
    projectList.replaceChildren(...visibleProjects.map(createProjectCard));
    if (visibleProjects.length === 0) {
        const emptyMessage = document.createElement('p');
        emptyMessage.textContent = 'Geen projecten gevonden voor deze categorie.';
        projectList.append(emptyMessage);
    }
}

function updateFilterState(buttons, category) {
    buttons.forEach((button) => {
        button.setAttribute('aria-pressed', String(button.dataset.filter === category));
    });
}

function initProjects() {
    const filters = document.querySelector('.project-filters');
    const projectList = document.querySelector('#project-list');
    if (!filters || !projectList) return;
    const buttons = filters.querySelectorAll('[data-filter]');
    const count = document.querySelector('#project-count');

    function updateProjects(category) {
        const visibleProjects = filterProjects(projects, category);
        renderProjects(projectList, visibleProjects);
        updateFilterState(buttons, category);
        count.textContent = visibleProjects.length + ' van ' + projects.length + ' projecten zichtbaar';
    }

    function handleFilterClick(event) {
        updateProjects(event.currentTarget.dataset.filter);
    }

    buttons.forEach((button) => button.addEventListener('click', handleFilterClick));
    updateProjects('all');
    filters.hidden = false;
    count.hidden = false;
}

function toggleBlogItem(content, button, title) {
    content.hidden = !content.hidden;
    const label = content.hidden ? 'Lees meer' : 'Lees minder';
    button.textContent = label;
    button.setAttribute('aria-expanded', String(!content.hidden));
    button.setAttribute('aria-label', label + ': ' + title);
}

function initBlogItem(article, index) {
    const content = article.querySelector('p:last-child');
    const title = article.querySelector('h3').textContent;
    const button = document.createElement('button');
    content.id = 'blog-content-' + (index + 1);
    content.hidden = true;
    button.type = 'button';
    button.className = 'blog-toggle';
    button.textContent = 'Lees meer';
    button.setAttribute('aria-expanded', 'false');
    button.setAttribute('aria-controls', content.id);
    button.setAttribute('aria-label', 'Lees meer: ' + title);

    function handleBlogClick() {
        toggleBlogItem(content, button, title);
    }

    button.addEventListener('click', handleBlogClick);
    content.before(button);
}

function validateField(name, value, typeMismatch = false) {
    value = value.trim();
    let error = '';
    if (name === 'name' && !value) {
        error = 'Vul je naam in. Alleen spaties zijn niet voldoende.';
    } else if (name === 'email') {
        if (!value) {
            error = 'Vul je e-mailadres in.';
        } else if (typeMismatch || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) {
            error = 'Vul een geldig e-mailadres in, bijvoorbeeld naam@voorbeeld.nl.';
        }
    } else if (name === 'message' && value.length < 10) {
        error = 'Schrijf een bericht van minimaal 10 tekens (zonder spaties aan het begin en einde).';
    }
    return error;
}

function showFieldError(field, error) {
    const errorElement = document.getElementById(field.name + '-error');
    errorElement.textContent = error;
    errorElement.hidden = !error;
    field.setAttribute('aria-invalid', String(Boolean(error)));
}

function checkContactField(field) {
    const error = validateField(field.name, field.value, field.validity.typeMismatch);
    showFieldError(field, error);
    return !error;
}

function initContactForm() {
    const contactForm = document.querySelector('#contact-form');
    if (!contactForm) return;
    const fields = [
        document.querySelector('#contact-name'),
        document.querySelector('#contact-email'),
        document.querySelector('#contact-message')
    ];
    const status = document.querySelector('#contact-status');
    let submitted = false;

    function handleContactSubmit(event) {
        // Geen server gekoppeld: voorkom navigatie en verstuur geen gegevens.
        event.preventDefault();
        submitted = true;
        const invalidFields = fields.filter((field) => !checkContactField(field));
        if (invalidFields.length) {
            status.className = 'form-status-error';
            status.textContent = 'Controleer de ' + invalidFields.length + ' gemarkeerde velden. Je bericht is niet verstuurd.';
            invalidFields[0].focus();
            return;
        }
        status.className = 'form-status-success';
        status.textContent = 'Bedankt! Alle velden zijn correct ingevuld.';
        contactForm.reset();
        submitted = false;
        fields.forEach((field) => field.removeAttribute('aria-invalid'));
    }

    function handleContactInput(event) {
        status.textContent = '';
        status.className = '';
        if (submitted) checkContactField(event.currentTarget);
    }

    contactForm.addEventListener('submit', handleContactSubmit);
    fields.forEach((field) => field.addEventListener('input', handleContactInput));
    contactForm.querySelector('fieldset').disabled = false;
}

initProjects();
document.querySelectorAll('[data-blog] article').forEach(initBlogItem);
initContactForm();
