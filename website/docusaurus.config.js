// @ts-check
import { themes as prismThemes } from 'prism-react-renderer';

/** @type {import('@docusaurus/types').Config} */
const config = {
  title: 'Owasp.Analyzers',
  tagline: 'Roslyn analyzers for the OWASP Top 10 — catch security issues at compile time',
  favicon: 'img/favicon.ico',

  url: 'https://marcelroozekrans.github.io',
  baseUrl: '/Owasp.Analyzers/',

  organizationName: 'MarcelRoozekrans',
  projectName: 'Owasp.Analyzers',

  onBrokenLinks: 'throw',
  onBrokenMarkdownLinks: 'warn',

  i18n: {
    defaultLocale: 'en',
    locales: ['en'],
  },

  presets: [
    [
      'classic',
      /** @type {import('@docusaurus/preset-classic').Options} */
      ({
        docs: {
          sidebarPath: './sidebars.js',
          editUrl: 'https://github.com/MarcelRoozekrans/Owasp.Analyzers/tree/main/website/',
        },
        blog: false,
        theme: {
          customCss: './src/css/custom.css',
        },
      }),
    ],
  ],

  themeConfig:
    /** @type {import('@docusaurus/preset-classic').ThemeConfig} */
    ({
      image: 'img/social-card.png',
      navbar: {
        title: 'Owasp.Analyzers',
        logo: {
          alt: 'Owasp.Analyzers Logo',
          src: 'img/logo.png',
        },
        items: [
          {
            type: 'docSidebar',
            sidebarId: 'docsSidebar',
            position: 'left',
            label: 'Docs',
          },
          {
            href: 'https://github.com/MarcelRoozekrans/Owasp.Analyzers',
            label: 'GitHub',
            position: 'right',
          },
          {
            href: 'https://www.nuget.org/packages/Owasp.Analyzers',
            label: 'NuGet',
            position: 'right',
          },
        ],
      },
      footer: {
        style: 'dark',
        links: [
          {
            title: 'Docs',
            items: [
              { label: 'Introduction', to: '/docs/intro' },
              { label: 'Getting Started', to: '/docs/getting-started/installation' },
              { label: 'Rules', to: '/docs/rules/a01-broken-access-control' },
            ],
          },
          {
            title: 'Community',
            items: [
              { label: 'GitHub', href: 'https://github.com/MarcelRoozekrans/Owasp.Analyzers' },
              { label: 'Issues', href: 'https://github.com/MarcelRoozekrans/Owasp.Analyzers/issues' },
            ],
          },
          {
            title: 'More',
            items: [
              { label: 'NuGet', href: 'https://www.nuget.org/packages/Owasp.Analyzers' },
              { label: 'OWASP Top 10', href: 'https://owasp.org/Top10/' },
              { label: 'License (MIT)', href: 'https://github.com/MarcelRoozekrans/Owasp.Analyzers/blob/main/LICENSE' },
            ],
          },
        ],
        copyright: `Copyright © ${new Date().getFullYear()} Marcel Roozekrans. Built with Docusaurus.`,
      },
      prism: {
        theme: prismThemes.github,
        darkTheme: prismThemes.dracula,
        additionalLanguages: ['csharp', 'bash', 'json', 'xml'],
      },
    }),
};

export default config;
