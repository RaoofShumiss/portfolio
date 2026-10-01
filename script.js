// Filter projecten en werk het aantal zichtbare resultaten bij.
const filters = document.querySelector('.project-filters');

if (filters) {
    const buttons = filters.querySelectorAll('[data-filter]');
    const projects = document.querySelectorAll('article[data-category]');
    const count = document.querySelector('#project-count');

    function filterProjects(category) {
        let visibleCount = 0;

        projects.forEach((project) => {
            project.hidden = category !== 'all' && project.dataset.category !== category;
            if (!project.hidden) visibleCount += 1;
        });

        buttons.forEach((button) => {
            button.setAttribute('aria-pressed', String(button.dataset.filter === category));
        });

        count.textContent = `${visibleCount} van ${projects.length} projecten zichtbaar`;
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
