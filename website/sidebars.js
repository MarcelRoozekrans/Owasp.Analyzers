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
        'rules/a02-security-misconfiguration',
        'rules/a03-software-supply-chain-failures',
        'rules/a04-cryptographic-failures',
        'rules/a05-injection',
        'rules/a06-insecure-design',
        'rules/a07-authentication-failures',
        'rules/a08-data-integrity',
        'rules/a09-logging-failures',
        'rules/a10-mishandling-exceptional-conditions',
      ],
    },
    'configuration',
    'taint-engine',
  ],
};

export default sidebars;
