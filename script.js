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

// Filter de gegevens en render de bijbehorende kaarten en het aantal resultaten.
const filters = document.querySelector('.project-filters');
const projectList = document.querySelector('#project-list');

if (filters && projectList) {
    const buttons = filters.querySelectorAll('[data-filter]');
    const count = document.querySelector('#project-count');

    function filterProjects(category) {
        const visibleProjects = projects.filter((project) =>
            category === 'all' || project.category === category
        );

        const cards = visibleProjects.map((project) => {
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
            technologies.append(label, ` ${project.technologies.join(', ')}`);

            article.append(title, description, technologies);
            return article;
        });

        projectList.replaceChildren(...cards);

        if (visibleProjects.length === 0) {
            const emptyMessage = document.createElement('p');
            emptyMessage.textContent = 'Geen projecten gevonden voor deze categorie.';
            projectList.append(emptyMessage);
        }

        buttons.forEach((button) => {
            button.setAttribute('aria-pressed', String(button.dataset.filter === category));
        });

        count.textContent = `${visibleProjects.length} van ${projects.length} projecten zichtbaar`;
    }

    buttons.forEach((button) => {
        button.addEventListener('click', () => filterProjects(button.dataset.filter));
    });

    filterProjects('all');
    filters.hidden = false;
    count.hidden = false;
}

// Maak blogteksten inklapbaar. Zonder JavaScript blijft alle tekst leesbaar.
document.querySelectorAll('[data-blog] article').forEach((article, index) => {
    const content = article.querySelector('p:last-child');
    const title = article.querySelector('h3').textContent;
    const button = document.createElement('button');

    content.id = `blog-content-${index + 1}`;
    content.hidden = true;
    button.type = 'button';
    button.className = 'blog-toggle';
    button.textContent = 'Lees meer';
    button.setAttribute('aria-expanded', 'false');
    button.setAttribute('aria-controls', content.id);
    button.setAttribute('aria-label', `Lees meer: ${title}`);

    button.addEventListener('click', () => {
        content.hidden = !content.hidden;
        const label = content.hidden ? 'Lees meer' : 'Lees minder';
        button.textContent = label;
        button.setAttribute('aria-expanded', String(!content.hidden));
        button.setAttribute('aria-label', `${label}: ${title}`);
    });

    content.before(button);
});

// Controleer alle contactvelden voordat het formulier wordt bevestigd.
const contactForm = document.querySelector('#contact-form');

if (contactForm) {
    const nameInput = document.querySelector('#contact-name');
    const emailInput = document.querySelector('#contact-email');
    const messageInput = document.querySelector('#contact-message');
    const status = document.querySelector('#contact-status');
    const fields = [nameInput, emailInput, messageInput];
    let submitted = false;

    function validateField(field) {
        const value = field.value.trim();
        let error = '';

        if (field === nameInput && !value) {
            error = 'Vul je naam in. Alleen spaties zijn niet voldoende.';
        } else if (field === emailInput) {
            if (!value) {
                error = 'Vul je e-mailadres in.';
            } else if (field.validity.typeMismatch || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) {
                error = 'Vul een geldig e-mailadres in, bijvoorbeeld naam@voorbeeld.nl.';
            }
        } else if (field === messageInput && value.length < 10) {
            error = 'Schrijf een bericht van minimaal 10 tekens (zonder spaties aan het begin en einde).';
        }

        const errorElement = document.getElementById(`${field.name}-error`);
        errorElement.textContent = error;
        errorElement.hidden = !error;
        field.setAttribute('aria-invalid', String(Boolean(error)));
        return !error;
    }

    contactForm.addEventListener('submit', (event) => {
        // Geen server gekoppeld: voorkom navigatie en verstuur geen gegevens.
        event.preventDefault();
        submitted = true;
        const invalidFields = fields.filter((field) => !validateField(field));

        if (invalidFields.length) {
            status.className = 'form-status-error';
            status.textContent = `Controleer de ${invalidFields.length} gemarkeerde velden. Je bericht is niet verstuurd.`;
            invalidFields[0].focus();
            return;
        }

        status.className = 'form-status-success';
        status.textContent = 'Bedankt! Alle velden zijn correct ingevuld.';
        contactForm.reset();
        submitted = false;
        fields.forEach((field) => field.removeAttribute('aria-invalid'));
    });

    fields.forEach((field) => {
        field.addEventListener('input', () => {
            status.textContent = '';
            status.className = '';
            if (submitted) validateField(field);
        });
    });

    // Activeer het formulier pas nadat de validatie is gekoppeld.
    contactForm.querySelector('fieldset').disabled = false;
}
