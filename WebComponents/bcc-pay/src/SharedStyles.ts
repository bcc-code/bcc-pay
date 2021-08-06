import { css } from 'lit-element';

export const cardSquareStyle = css`
  box-sizing: border-box;
  display: grid;
  grid-template-columns: 1fr;
  grid-auto-rows: max-content;
  grid-row-gap: 1rem;
  max-width: 478px;
  background: #ffffff;
  box-shadow: 0px 24px 25px -10px rgba(182, 194, 240, 0.54);
  border-radius: 15px;
  padding: 55px 74px;
`;

export const cardTitleStyle = css`
  font-size: 1.375rem;
  font-weight: 700;
`;

export const cardSubtitleStyle = css`
  font-size: 18px;
  text-transform: uppercase;
  color: var(--neutral-500);
`;

export const cardPriceStyle = css`
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 27px 30px;
  background-color: hsl(238, 90%, 99%);
  border: 1px solid hsl(227, 65%, 95%);
  border-radius: 10px;
`;

export const cardTagStyle = css`
  font-size: 1.125rem;
  font-weight: 900;
`;

export const cardCostStyle = `
font-size: 1.125rem;
font-weight: 900;
`;

export const nestButtonStyle = css`
  font-size: 1rem;
  text-transform: uppercase;
  background: hsl(258, 71%, 61%);
  color: white;
  padding: 23px;
  border-radius: 5px;
  text-align: center;
  font-weight: 700;
  margin-top: 1rem;
`;

export const linkButtonStyle = css`
  background: none !important;
  border: none;
  padding: 0 !important;
  /*optional*/
  font-family: arial, sans-serif;
  /*input has OS specific font-family*/
  color: #069;
  text-decoration: underline;
  cursor: pointer;
`;

export const textInputStyle = css`
  input[type='text'] {
    width: 100%;
    border: 2px solid #aaa;
    border-radius: 4px;
    margin: 8px 0;
    outline: none;
    padding: 8px;
    box-sizing: border-box;
    transition: 0.3s;
  }

  input[type='text']:focus {
    border-color: dodgerBlue;
    box-shadow: 0 0 8px 0 dodgerBlue;
  }
`;

export function applyStyles() {
  const cardSquare = document.getElementsByClassName(
    'card-square'
  )[0] as HTMLElement;
  cardSquare.style.cssText = cardSquareStyle.toString();

  const cardTitle = document.getElementsByClassName(
    'card-title'
  )[0] as HTMLElement;
  cardTitle.style.cssText = cardTitleStyle.toString();

  const cardSubtitle = document.getElementsByClassName(
    'card-subtitle'
  )[0] as HTMLElement;
  cardSubtitle.style.cssText = cardSubtitleStyle.toString();

  const cardPrice = document.getElementsByClassName(
    'card-price'
  )[0] as HTMLElement;
  cardPrice.style.cssText = cardPriceStyle.toString();

  const cardCost = document.getElementsByClassName(
    'card-cost'
  )[0] as HTMLElement;
  cardCost.style.cssText = cardCostStyle.toString();

  const cardTag = document.getElementsByClassName('card-tag')[0] as HTMLElement;
  cardTag.style.cssText = cardTagStyle.toString();

  const netsButton = document.getElementsByClassName(
    'nets-button'
  )[0] as HTMLElement;
  netsButton.style.cssText = nestButtonStyle.toString();

  const reloadButtons = document.getElementsByClassName('reload-button');
  Array.from(reloadButtons).forEach(element => {
    const htmlElement = element as HTMLElement;
    htmlElement.style.cssText = nestButtonStyle.toString();
  });

  const linkButtons = document.getElementsByClassName('link-button');
  Array.from(linkButtons).forEach(element => {
    const htmlElement = element as HTMLElement;
    htmlElement.style.cssText = linkButtonStyle.toString();
  });

  const inputStyle = document.createElement('style');
  inputStyle.innerHTML = textInputStyle.toString();
  document.head.appendChild(inputStyle);
}
