/** @type {import('@docusaurus/types').DocusaurusConfig} */
module.exports = {
  title: "Bunny Mod",
  url: 'https://Freiling87.github.com',
  baseUrl: '/BunnyMod/',
  onBrokenLinks: 'throw',
  onBrokenMarkdownLinks: 'warn',
  favicon: 'img/favicon.ico',
  organizationName: 'Freiling87',
  projectName: 'BunnyMod',
  themeConfig: {
    hideableSidebar: true,
    prism: {
      theme: require('prism-react-renderer/themes/dracula'),
      additionalLanguages: ['clike', 'csharp', 'bash'],
    },
    navbar: {
      hideOnScroll: true,
      title: "Bunny Mod",
      logo: {
        alt: 'A bunny in the bottom-left corner on a background of a burning house',
        src: 'img/logo.png',
      },
      items: [
        {
          type: 'doc',
          docId: 'intro',
          position: 'left',
          label: 'Introduction',
        },
        {
          href: 'https://github.com/Freiling87/BunnyMod',
          position: 'right',
          className: 'header-github-link',
          'aria-label': 'GitHub repository',
        },
      ],
    },
    footer: {
      style: 'dark',
      links: [
        {
          title: 'Docs',
          items: [
            {
              label: 'Introduction',
              to: '/docs/intro',
            },
          ],
        },
        {
          title: 'Community',
          items: [
            
          ],
        },
        {
          title: 'More',
          items: [
            {
              label: 'GitHub',
              href: 'https://github.com/Freiling87/BunnyMod',
            },
          ],
        },
      ],
      copyright: `Copyright © ${new Date().getFullYear()} Bunny Mod. Built with Docusaurus.`,
    },
  },
  presets: [
    [
      '@docusaurus/preset-classic',
      {
        docs: {
          sidebarPath: require.resolve('./sidebars.js'),
          editUrl:
            'https://github.com/Freiling87/BunnyMod/edit/master/website/',
        },
        blog: {
          showReadingTime: true,
          editUrl:
            'https://github.com/Freiling87/BunnyMod/edit/master/website/blog/',
        },
        theme: {
          customCss: require.resolve('./src/css/custom.css'),
        },
      },
    ],
  ],
};
