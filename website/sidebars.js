/** @type {import('@docusaurus/plugin-content-docs').SidebarsConfig} */
const sidebars = {
  docsSidebar: [
    'intro',
    {
      type: 'category',
      label: 'Getting Started',
      items: ['getting-started/installation', 'getting-started/quick-start'],
    },
    {
      type: 'category',
      label: 'Rules',
      items: [
        'rules/a01-broken-access-control',
        'rules/a02-cryptographic-failures',
        'rules/a03-injection',
        'rules/a04-insecure-design',
        'rules/a05-security-misconfiguration',
        'rules/a06-vulnerable-components',
        'rules/a07-authentication-failures',
        'rules/a08-data-integrity',
        'rules/a09-logging-failures',
        'rules/a10-ssrf',
      ],
    },
    'configuration',
    'taint-engine',
  ],
};

export default sidebars;
