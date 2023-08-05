const express = require("express");
const body_parser = require("body-parser");
const axios = require("axios");
const url = "https://graph.facebook.com/v15.0/";
const app = express().use(body_parser.json());
require("dotenv").config();
const port = process.env.PORT;
//const { res } = require("./openai_api");

//////////////////////////////////
const { Configuration, OpenAIApi } = require("openai");
const configuration = new Configuration({
  apiKey: process.env.OPENAI_API_KEY,
});
const openai = new OpenAIApi(configuration);
///////////////////////////////////

const token = process.env.TOKEN;
const myToken = process.env.MYTOKEN;

app.get("/", async (req, res) => {
  console.log("Called for open page...");
  res.status(200).send("Everything is fine...");
});

app.get("/webhooks", (req, res) => {
  /*let mode = req.query["hub.mode"];
    let challenge = req.query["hub.challenge"];
    let token_r = req.query["hub.verify_token"];
    console.log("there's a get request");


    if (mode && token_r)
    {
        if ((mode == "subscribe") && (token_r == myToken))
        {
            res.status(200).send(challenge);    
        }
        else 
        {
            res.sendStatus(403);
        }
    }*/
  console.log("called webhooks page...");
  let challenge = req.query["hub.challenge"];
  console.log(challenge);
  res.status(200).send(challenge);
});

app.post("/webhooks", async (req, res) => {
  if (
    req.body.entry &&
    req.body.entry[0].changes &&
    req.body.entry[0].changes[0] &&
    req.body.entry[0].changes[0].value.messages &&
    req.body.entry[0].changes[0].value.messages[0]
  ) {
    let body_param = req.body;
    //console.log(JSON.stringify(body_param, null, 2));

    let phone_no_id =
      body_param.entry[0].changes[0].value.metadata.phone_number_id;
    let from = body_param.entry[0].changes[0].value.messages[0].from;
    let msg = body_param.entry[0].changes[0].value.messages[0].text.body;
    console.log(`Query from : ${from}`)
    console.log(`Query : ${msg}`);

    const response = await openai
      .createCompletion({
        model: "text-davinci-003",
        prompt: msg,
        max_tokens: 150,
        temperature: 0,
      })
      /*.then((resu) => {
        console.log("Inside then function...");
      })
      .catch((err) => {
        console.log("Inside err function...", err);
      });*/
    const result = response.data.choices[0].text;
    console.log("The response is : ",result);
    //const result2 = res(msg);
    //console.log(result2);
    var config = {
      method: "post",
      url: `${url}${phone_no_id}/messages`,
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      data: {
        messaging_product: "whatsapp",
        recipient_type: "individual",
        to: from,
        type: "text",
        text: {
          preview_url: false,
          body: result
        },
      },
    };
    axios(config)
      .then((result) => {
        console.log("Sent a message to whatsapp to number :",from);
        res.status(200).send("everything went fine...");
      })
      .catch((err) => {
        res.status(500).send("error machan...");
      });
  }
});

app.listen(process.env.PORT || 4200, () => {
  console.log(`Webhook is listening....`);
});
