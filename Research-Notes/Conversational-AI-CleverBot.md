# Conversational AI Tech Review
This is a research note of the technologies behind current conversational AI. The main purpose of this analysis are as follows:
- Evaluate the performance of current conversational AI in the context of our main product space (language learning) - **Can conversational UI handle relatively complex conversations?**
- Decide if the technology is suitable for the team given resources, time, etc. - **As a team, (if current solution sucks) are we capable of developing conversational AI for language learning?**
- Analyze if the technologies and existing solutions can be adopted into our team VR project within limited time horizon. - **Can we leverage existing solutions?**

Chen Chen
January 12, 2017

# Existor's Language Model
Apparently, most of the language models are relying on neural networks like **RNN (Recurrent Neural Networks)** to learn word sequence probabilities. Most recently, two popular language model algorithms -  [word2vec](https://www.tensorflow.org/tutorials/word2vec/) and [GloVe](http://nlp.stanford.edu/projects/glove/) brings language models to the next level. Instead of computing conditional probabilities on word sequence or occurrence, these kind of algorithm will represent words into *word vectors* to help classify the meanings they represent. Further research are focusing on even higher level of abstraction (**because generally accuracy and robustness will benefit from the level of abstraction**) by converting sentence and even [paragraph](https://arxiv.org/pdf/1405.4053v2.pdf) into feature vectors, which can be used to solve context analogies **mathematically**.

One of their leading product - the CleverBot has a pretty interesting way of handling language interpretation. Their engine (at least based on [1]) doesn't really interpret any context meaning of user input. Their algorithm uses some type of string similarity matching algorithm to search for "similar" sentence in their database which have billions of historical user interaction data. Once mapped to a specific historical input, the engine simply choose one among all the responses to that old input. It is not surprising that over time the engine behaves more "human" because **you are getting responses from an actual human who interacted with the bot before!**.

### Feasibility of the Model
The general truth of machine learning methods is that they are build upon **big big** dataset. In order for a general purpose conversational AI to reach peak performance, one might consider train it over the **100 billion word Google News corpus** [1]. Existor claim that their [Cleverbot](http://www.cleverbot.com/) was trained on a much lighter dataset of **279 million interactions** within the **1.4 billion conversational interactions and approximately 9 billion individual words**.

Good news is that their API allows some high-level interaction with their language interpretation model but not sure to what degree. We don't know much percentage of their 1.4 billion or even the 279 million interactions database is available to the API , which plays a big factor in the performance.

The key questions yet to be answered here are:

1. How open is [CleverBot's API](http://www.cleverscript.com/)? Do we have access to their model and database?
2. Is the API and service lightweight enough to be hosted on a VR program that runs on low-end mobile platform (e.g. GearVR)?
3. **Is the model suited for our language learning scenario given most of its database are probably casual conversation?**
4. **Will the model-engaged conversation facilitate language learning? And will people use it?**

# References
1. [**[Webpage]** Existor's article about current conversational AIs](http://www.existor.com/products/cleverbot-data-for-machine-learning/)
2. [**[Webpage]** CleverScript Description](http://www.cleverscript.com/about/)
3. [**[Research Article]** Distributed Representation of Sentences and Documents](https://arxiv.org/pdf/1405.4053v2.pdf)
4. [**[Research Article]** A Neural Conversational Model](https://arxiv.org/pdf/1506.05869v1.pdf)
5. [**[Webpage]** How bots will change the Web, according to a bot we built to answer that question](https://www.washingtonpost.com/news/the-intersect/wp/2016/04/15/how-bots-will-change-the-web-according-to-a-bot-we-built-to-answer-that-question/?utm_term=.2d7ac8c86265)
